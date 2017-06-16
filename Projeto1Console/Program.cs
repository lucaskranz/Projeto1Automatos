using Projeto1Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto1Console
{
    class AutomatoTabela
    {
        public string token;
        public Estado estadoOrigem;
        public Estado estadoDestino;

        public AutomatoTabela(string token, Estado estadoOrigem, Estado estadoDestino)
        {
            this.token = token;
            this.estadoOrigem = estadoOrigem;
            this.estadoDestino = estadoDestino;
        }
    }

    class Estado
    {
        public string nome;
        public bool inicial;
        public bool final;

        public Estado(string nome, bool inicial, bool final)
        {
            this.nome = nome;
            this.inicial = inicial;
            this.final = final;
        }
    }

    class Linha
    {
        public string texto;
        public Estado estadoOrigem;
    }
    
    class Automato
    {
        public int counter;
        Linha linha = new Linha();
        public List<AutomatoTabela> AutomatoTabela = new List<AutomatoTabela>();
        public List<String> tokens = new List<string>();
        public List<Estado> estados = new List<Estado>();

        public void GerarAutomato()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"D:\Projetos\Projeto1Automatos\Projeto1Console\entrada.txt");
            
            while ((linha.texto = file.ReadLine()) != null)
            {
                if (linha.texto == "")
                    break;
                counter = 0;
                linha.texto = linha.texto.Replace(" ", "");
                while (counter < linha.texto.Length)
                {
                    if (linha.texto[0] == '<') //gramaticas regulares
                        CriarTokensGramatica();
                    else //tokens
                        criarTokens();
                    counter++;
                }
            }
            estados.Add(new Estado(nome:"X", inicial:false, final:true));
            file.Close();
        }

        public void criarTokens()
        {
            if (linha.texto[counter] == ';')
                return;
            else if (AutomatoTabela.Count == 0 || counter == 0)
                AutomatoTabela.Add(new AutomatoTabela(token: linha.texto[counter].ToString(), estadoOrigem: CriarEstadosTokens(origem: true), estadoDestino: CriarEstadosTokens(origem: false)));
            else
                AutomatoTabela.Add(new AutomatoTabela(token: linha.texto[counter].ToString(), estadoOrigem: AutomatoTabela[AutomatoTabela.Count - 1].estadoDestino, estadoDestino: CriarEstadosTokens(origem: false)));
            
            if (!ExisteToken(linha.texto[counter].ToString()) && linha.texto[counter] != ';')
                tokens.Add(linha.texto[counter].ToString());
        }

        public Estado CriarEstadosTokens(bool origem)
        {
            Estado estado;
           
            if (estados.Count == 0)
                estado = new Estado(nome: "S", inicial: true, final: false);
            else if (estados.Count == 1)
                estado = new Estado(nome: "A", inicial: false, final: false);
            else
            {
                if (counter == 0 && origem)
                    return estados[0];
                else
                {
                    char p = Convert.ToChar(estados[estados.Count - 1].nome);
                    p++;
                    if (p == 'S' || p == 'X')
                        p++;
                    estado = new Estado(nome: p.ToString(), inicial: false, final: false);
                }
            }

            estados.Add(estado);
            return estado;
        }

        public void CriarTokensGramatica()
        {
            int index, aux = 0;
            string token  = "";
            Estado estado = new Estado(null, false, false);
            
            if (counter == 0)
            {
                for (index = 1; index < linha.texto.Length; index++)
                {
                    if (linha.texto[counter + index] == '>')
                        break;
                }
                linha.estadoOrigem = CriarEstadosGramatica(linha.texto.Substring(counter + 1, index - 1));
            }
            else if(linha.texto[counter] == '=' || linha.texto[counter] == '|')
            {
                for (index = 1; (index + counter) < linha.texto.Length; index++)
                {
                    if ((linha.texto[counter + index] == '<') && (aux == 0))
                    {
                        token = linha.texto.Substring(counter + 1, index - 1);
                        aux = counter + index;
                        index = 0;
                    }
                    else if (aux != 0 )
                    {
                        if (linha.texto[aux + index] == '>')
                        {
                            estado = new Estado(nome: linha.texto.Substring(aux + 1, index - 1), inicial: false, final: false);
                            break;
                        }
                    }
                    else if (linha.texto[counter + index] == '|')
                    {
                        token = linha.texto.Substring(counter + 1, index - 1);
                        estado = CriarEstadosTokens(false);
                        break;
                    }
                }
                if(! string.IsNullOrEmpty(token)) 
                    AutomatoTabela.Add(new AutomatoTabela(token: token, estadoOrigem: linha.estadoOrigem, estadoDestino: estado));

                if (! ExisteToken(token))
                    tokens.Add(token);
            }
        }

        public bool ExisteToken(string token)
        {
            if (token.Equals('|') || token.Equals(';') || token.Equals('=') || string.IsNullOrEmpty(token))
                return true;

            for (int i = 0; i < tokens.Count; i++)
                if (tokens[i].Equals(token))
                    return true;
            return false;
        }

        public bool ExisteEstado(string estado)
        {
            for (int i = 0; i < estados.Count; i++)
                if (estados[i].nome.Equals(estado))
                    return true;
            return false;
        }

        public Estado CriarEstadosGramatica(string estado)
        {
            if (!ExisteEstado(estado))
                estados.Add(new Estado(nome: estado, inicial: false, final: false));

            return (new Estado(nome: estado, inicial: false, final: false));
        }
    }

    class Program
    {
        static void Main()
        {
            Automato aut = new Automato();
            aut.GerarAutomato();
            int numEstados = aut.estados.Count + 1, numTokens = aut.tokens.Count + 2;
            string estadoDestino, estadoOrigem;
            string token, estado;

            string [,] matriz = new string [numEstados, numTokens];

            for(int linha = 0; linha < numEstados; linha++)
            { 
                for (int col = 0; col < numTokens; col++)
                {
                    if (linha == 0)
                    {
                        if (col == 0)
                            matriz[linha, col] = "";
                        else if (col > aut.tokens.Count)
                            matriz[linha, col] = "x";
                        else
                            matriz[linha, col] = aut.tokens[col - 1];
                    }
                    else
                    {
                        if (col == 0)
                        {
                            if (linha <= aut.estados.Count)
                                matriz[linha, col] = aut.estados[linha - 1].nome;
                        }
                        else
                        {
                            token = matriz[0, col];
                            estado = matriz[linha, 0];
                            matriz[linha, col] = "X";
                            for (int x = 0; x < aut.AutomatoTabela.Count; x++)
                            {
                                if (aut.AutomatoTabela[x].estadoOrigem != null)
                                {
                                    if (aut.AutomatoTabela[x].token.Equals(token) && aut.AutomatoTabela[x].estadoOrigem.nome.Equals(estado))
                                    {   
                                        if (matriz[linha, col] == "X")
                                            matriz[linha, col] = aut.AutomatoTabela[x].estadoDestino.nome;
                                        else
                                            matriz[linha, col] += aut.AutomatoTabela[x].estadoDestino.nome;
                                    }
                                }
                            }                            
                        }
                    } 
                }
            }

            string texto = "";
            for (int linha = 0; linha < numEstados; linha++)
            {
                texto = "";
                for (int col = 0; col < numTokens; col++)
                {
                    if (linha == 0)
                        texto += " |" + matriz[linha, col] + "| ";
                    else
                        if (matriz[linha, col].Length > 1)
                            texto += " " + matriz[linha, col] + "  ";
                        else
                            texto += "  " + matriz[linha, col] + "  ";
                }
                System.Console.WriteLine(texto);
            }

            for (int i = 0; i < aut.AutomatoTabela.Count; i++)
            {
                if (aut.AutomatoTabela[i].estadoDestino == null)
                    estadoDestino = "-";
                else
                    estadoDestino = aut.AutomatoTabela[i].estadoDestino.nome;

                if (aut.AutomatoTabela[i].estadoOrigem == null)
                    estadoOrigem = "-";
                else
                    estadoOrigem = aut.AutomatoTabela[i].estadoOrigem.nome;
                
                System.Console.WriteLine("| token: " + aut.AutomatoTabela[i].token +
                                         "| estadoOrigem: " + estadoOrigem + 
                                         "| estadoDestino: " + estadoDestino);
            }

            System.Console.ReadLine();
        }
    }
}

