using Projeto1Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projeto1Console
{
    class AutomatoFinito 
    {
        public string token;
        public List<Estado> estadoOrigem;
        public List<Estado> estadoDestino;        
    }
   
    class Estado
    {
        public string nome;
        public bool inicial;
        public bool final;

        public Estado() { }

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
        public List<AutomatoFinito> TabelaAFND = new List<AutomatoFinito>(); //automato finito nao deterministico
        public List<AutomatoFinito> TabelaAFD = new List<AutomatoFinito>();    //automato finito deterministico
        public List<string> tokens = new List<string>();
        public List<string> AuxEstados = new List<string>();
        public List<Estado> estados = new List<Estado>();
        public List<Estado> estadosAFD = new List<Estado>();
        public List<string> estadosFim = new List<string>();
        System.IO.StreamWriter saida = new System.IO.StreamWriter(@"saida.txt"); 
        System.IO.StreamReader file = new System.IO.StreamReader(@"entrada.txt");

        public void GerarAutomatoNaoDeterministico()
        {            
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
            
            file.Close();
        }

        public void DeterminizarAutomato()
        {
            CriacaoNovoEstadoDeterministico();
            tokens.Sort();
            AutomatoFinito afd;

            for (int i = 0; i < TabelaAFND.Count; i++)
            {
                afd = new AutomatoFinito();
                afd.estadoDestino = new List<Estado>();
                afd.estadoOrigem = new List<Estado>();
                string estadoOrigem = "";

                for (int b = 0; b < TabelaAFND[i].estadoOrigem.Count; b++)
                    estadoOrigem += TabelaAFND[i].estadoOrigem[b].nome;

                if (AuxEstados.Contains(estadoOrigem))
                {
                    string estado = "";
                    bool final = false;

                    afd.token = TabelaAFND[i].token;
                    afd.estadoOrigem = TabelaAFND[i].estadoOrigem;
                    afd.estadoDestino = TabelaAFND[i].estadoDestino;

                    for (int j = 0; j < TabelaAFND[i].estadoOrigem.Count; j++)
                    {
                        estado += TabelaAFND[i].estadoOrigem[j].nome;

                        if (!final)
                            final = TabelaAFND[i].estadoOrigem[j].final;
                    }
                    if (!ExisteEstadoAFD(estado))
                        estadosAFD.Add(new Estado(nome: estado, inicial: false, final: final));
                    estado = "";
                    final = false;

                    for (int j = 0; j < TabelaAFND[i].estadoDestino.Count; j++)
                    {
                        estado += TabelaAFND[i].estadoDestino[j].nome;

                        if (!final)
                            final = TabelaAFND[i].estadoDestino[j].final;
                    }
                    if (!ExisteEstadoAFD(estado))
                        estadosAFD.Add(new Estado(nome: estado, inicial: false, final: final));

                    
                    TabelaAFD.Add(afd);
                }
            }
            estadosAFD.Add(new Estado(nome: "X", inicial: false, final: true));
        }
        
        private void CriacaoNovoEstadoDeterministico()
        {
            AutomatoFinito afd = new AutomatoFinito();
            AuxEstados.Add("S");
            string estadoOrigem = "";
            string estadoDestino = "";
            bool Existe = false;

            for (int i = 0; i < TabelaAFND.Count; i++)
            {
                afd = new AutomatoFinito();
                afd.estadoDestino = new List<Estado>();
                afd.estadoOrigem = new List<Estado>();
                TabelaAFND[i].estadoOrigem = TabelaAFND[i].estadoOrigem.OrderBy(l => l.nome).ToList(); ;
                TabelaAFND[i].estadoDestino = TabelaAFND[i].estadoDestino.OrderBy(l => l.nome).ToList(); ;

                if (TabelaAFND[i].estadoDestino.Count > 1)
                {
                    afd.token = TabelaAFND[i].token;
                    afd.estadoOrigem = TabelaAFND[i].estadoOrigem;
                    afd.estadoDestino = TabelaAFND[i].estadoDestino;

                    AutomatoFinito automato;
                    for (int j = 0; j < tokens.Count; j++)
                    {
                        automato = new AutomatoFinito();
                        automato.estadoDestino = new List<Estado>();
                        automato.estadoOrigem = new List<Estado>();

                        for (int n = 0; n < afd.estadoDestino.Count; n++)
                        {
                            for (int x = 0; x < TabelaAFND.Count; x++)
                            {
                                estadoOrigem = "";

                                for (int b = 0; b < TabelaAFND[x].estadoOrigem.Count; b++)
                                    estadoOrigem += TabelaAFND[x].estadoOrigem[b].nome;

                                if ((afd.estadoDestino[n].nome == estadoOrigem) && (tokens[j] == TabelaAFND[x].token))
                                {
                                    estadoDestino = "";
                                    for (int a = 0; a < TabelaAFND[x].estadoDestino.Count; a++)
                                    {
                                        Existe = false;
                                        for (int z = 0; z < automato.estadoDestino.Count; z++)
                                            if (automato.estadoDestino[z].nome == TabelaAFND[x].estadoDestino[a].nome)
                                            {
                                                Existe = true;
                                                break;
                                            }
                                        if (Existe)
                                            break;
                                        automato.estadoDestino.Add(TabelaAFND[x].estadoDestino[a]);
                                        estadoDestino += TabelaAFND[x].estadoDestino[a].nome;
                                        automato.estadoDestino = automato.estadoDestino.OrderBy(l => l.nome).ToList();
                                    }
                                    n++;
                                    x = -1;
                                    if (n == afd.estadoDestino.Count)
                                        break;
                                }
                            }
                        }
                        automato.token = tokens[j];
                        automato.estadoOrigem = afd.estadoDestino;
                        Existe = false;
                        string auxDestino, auxOrigem;
                        estadoDestino = "";
                        estadoOrigem = "";

                        for (int b = 0; b < automato.estadoOrigem.Count; b++)
                            estadoOrigem += automato.estadoOrigem[b].nome;

                        for (int b = 0; b < automato.estadoDestino.Count; b++)
                            estadoDestino += automato.estadoDestino[b].nome;

                        if (automato.estadoDestino.Count != 0 && automato.estadoOrigem.Count != 0)
                        {
                            for (int y = 0; y < TabelaAFND.Count; y++)
                            {
                                auxDestino = "";
                                auxOrigem = "";

                                for (int b = 0; b < TabelaAFND[y].estadoOrigem.Count; b++)
                                    auxOrigem += TabelaAFND[y].estadoOrigem[b].nome;

                                for (int b = 0; b < TabelaAFND[y].estadoDestino.Count; b++)
                                    auxDestino += TabelaAFND[y].estadoDestino[b].nome;

                                if (automato.token == TabelaAFND[y].token && estadoDestino == auxDestino && estadoOrigem == auxOrigem)
                                {
                                    Existe = true;
                                    break;
                                }
                            }
                            if (!Existe)
                            {
                                TabelaAFND.Add(automato);
                            }
                        }
                        else
                            continue;                                                                     
                                               
                        estadoDestino = "";

                        for (int b = 0; b < automato.estadoDestino.Count; b++)
                            estadoDestino += automato.estadoDestino[b].nome;
                                               
                        if (estadoDestino.Length > 0)
                            AuxEstados.Add(estadoDestino);
                    }
                }
                else
                {
                    AuxEstados.Add(TabelaAFND[i].estadoDestino[0].nome);
                }
            }
        }

        public void criarTokens()
        {
            var afnd = new AutomatoFinito();
            bool Existe = false;
            int n = 0;
            afnd.estadoOrigem = new List<Estado>();

            if (linha.texto[counter] == ';')
            {
                estados[estados.Count - 1].final = true;
                return;
            }
            else if (TabelaAFND.Count == 0 || counter == 0)
            {
                afnd.token = linha.texto[counter].ToString();
                afnd.estadoOrigem.Add(CriarEstadosTokens(origem: true, final: false));

                for (n = 0; n < TabelaAFND.Count && !Existe; n++)
                    for (int x = 0; x < TabelaAFND[n].estadoOrigem.Count; x++)
                        if ((TabelaAFND[n].token == afnd.token) && (TabelaAFND[n].estadoOrigem[x].nome == afnd.estadoOrigem[afnd.estadoOrigem.Count - 1].nome))
                        {
                            Existe = true;
                            break;
                        }
            }
            else
            {
                afnd.token = linha.texto[counter].ToString();
                afnd.estadoOrigem.Add(estados[estados.Count - 1]);

                for (n = 0; n < TabelaAFND.Count && !Existe; n++)
                    for (int x = 0; x < TabelaAFND[n].estadoOrigem.Count; x++)
                        if ((TabelaAFND[n].token == afnd.token) && (TabelaAFND[n].estadoOrigem[x].nome == afnd.estadoOrigem[afnd.estadoOrigem.Count - 1].nome))
                        {
                            Existe = true;
                            break;
                        }
            }

            if (Existe)
            {
                afnd.estadoDestino = new List<Estado>();
                TabelaAFND[n-1].estadoDestino.Add(CriarEstadosTokens(origem: false, final: false));
            }
            else
            {
                afnd.estadoDestino = new List<Estado>();
                afnd.estadoDestino.Add(CriarEstadosTokens(origem: false, final:false));
                TabelaAFND.Add(afnd);
            }

            if (!ExisteToken(linha.texto[counter].ToString()) && linha.texto[counter] != ';')
                tokens.Add(linha.texto[counter].ToString());
        }

        public Estado CriarEstadosTokens(bool origem, bool final)
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
                    if (estados.Count > 24)
                    {
                        int i = estados.Count - 24;
                        char p = Convert.ToChar(estados[i].nome[0]);
                        estado = new Estado(nome: p.ToString() + "1", inicial: false, final: false);
                    }
                    else
                    {
                        char p;
                        for (int i = 1; true; i++)
                        {
                            try
                            {
                                p = Convert.ToChar(estados[estados.Count - i].nome);
                                break;
                            }
                            catch
                            {
                                continue;
                            }                                     
                        }

                        p++;
                        if (p == 'S' || p == 'X')
                            p++;
                        estado = new Estado(nome: p.ToString(), inicial: false, final: false);
                    }
                }
            }
            estado.final = final;

            if (!ExisteEstado(estado.nome))
                estados.Add(estado);

            return estado;
        }

        public void CriarTokensGramatica()
        {
            int index, aux = 0, n = 0;
            string token  = "";
            Estado estado = new Estado(null, false, false);
            var afnd = new AutomatoFinito();
            bool Existe = false;
            afnd.estadoDestino = new List<Estado>();
            afnd.estadoOrigem = new List<Estado>();

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
                        string nome = ""; ;
                        token = linha.texto.Substring(counter + 1, index - 1);

                        if (token.Equals("&"))
                            break;

                        estadosFim.Add(token);

                        for (int i = 0; i < estadosFim.Count; i++)
                            nome += "'";

                        estado = new Estado(nome:"@"+nome, inicial: false, final: true); 
                        estados.Add(estado);
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(token) && token != "&")
                {
                    afnd.token = token;
                    afnd.estadoOrigem.Add(linha.estadoOrigem);
                    for (n = 0; n < TabelaAFND.Count && !Existe; n++)
                        for (int x = 0; x < TabelaAFND[n].estadoOrigem.Count; x++)
                            if ((TabelaAFND[n].token == afnd.token) && (TabelaAFND[n].estadoOrigem[x].nome == linha.estadoOrigem.nome))
                            {
                                Existe = true;
                                break;
                            }

                    if (Existe)
                        TabelaAFND[n-1].estadoDestino.Add(estado);
                    else
                    {
                        
                        afnd.estadoDestino.Add(estado);
                        TabelaAFND.Add(afnd);
                    }
                }

                if (! ExisteToken(token))
                    tokens.Add(token);
            }
        }

        public bool ExisteToken(string token)
        {
            if (token.Equals('|') || token.Equals(';') || token.Equals('=') || token.Equals("&") || string.IsNullOrEmpty(token))
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

        public bool ExisteEstadoAFD(string estado)
        {
            for (int i = 0; i < estadosAFD.Count; i++)
                if (estadosAFD[i].nome.Equals(estado))
                    return true;
            return false;
        }

        public Estado CriarEstadosGramatica(string estado)
        {
            bool final = false;

            if (linha.texto.Contains("&"))
                final = true;

            if (!ExisteEstado(estado))
            {
                if(estado.Equals("S"))
                    estados.Add(new Estado(nome: estado, inicial: true, final: final));
                else
                    estados.Add(new Estado(nome: estado, inicial: false, final: final));
            }

            return (new Estado(nome: estado, inicial: false, final: final));
        }
        
        public void GeraMatriz()
        {

            string[,] matriz = new string[(estadosAFD.Count + 1), (tokens.Count + 3)];
            string token, estado, conteudoLinha="";
            saida.WriteLine("AUTOMATO DETERMINISTICO");
            saida.AutoFlush = true;

            //Carregar Matriz AFND
            for (int linha = 0; linha < (estadosAFD.Count + 1); linha++)
            {
                conteudoLinha = "";
                for (int col = 0; col < (tokens.Count + 3); col++)
                {                    
                    if (linha == 0)
                    {
                        if (col == 0)
                            matriz[linha, col] = "inicial/final";
                        else if (col == 1)
                            matriz[linha, col] = "estados";
                        else if (col-1 > tokens.Count)
                            matriz[linha, col] = "x";
                        else if (col >=2)
                            matriz[linha, col] = tokens[col - 2];
                    }
                    else
                    {
                        if (col == 1)
                        {
                            if (linha <= estadosAFD.Count)
                            {
                                if (estadosAFD[linha - 1].inicial)
                                    matriz[linha, col-1] += ">";
                                if (estadosAFD[linha - 1].final)
                                    matriz[linha, col-1] += "*";


                                if (matriz[linha, col - 1] == null)
                                    matriz[linha, col - 1] = " ";

                                matriz[linha, col] = estadosAFD[linha - 1].nome;

                            }
                        }
                        else if (col > 1)
                        {
                            token = matriz[0, col];
                            estado = matriz[linha, 1];
                            string eOrigem, eDestino;

                            for (int x = 0; x < TabelaAFD.Count; x++)
                            {
                                eOrigem = "";
                                eDestino = "";
                                for (int j = 0; j < TabelaAFD[x].estadoOrigem.Count; j++)
                                    eOrigem += TabelaAFD[x].estadoOrigem[j].nome;

                                if (TabelaAFD[x].token.Equals(token) && eOrigem.Equals(estado))
                                {
                                    for (int h = 0; h < TabelaAFD[x].estadoDestino.Count; h++)
                                        eDestino += TabelaAFD[x].estadoDestino[h].nome;

                                    matriz[linha, col] = eDestino;
                                    break;
                                }
                                else
                                    matriz[linha, col] = "x";

                            }
                        }
                    }
                    if(col == 1)
                        conteudoLinha = matriz[linha, col-1] + ",";
                    conteudoLinha += matriz[linha, col] + ",";
                }
                saida.WriteLine(conteudoLinha);
                saida.AutoFlush = true;
            }
        }

        public void GeraMatrizAFND()
        {
            string[,] matriz = new string[(estados.Count + 1), (tokens.Count + 3)];
            string token, estado;
            string conteudoLinha;
            //Carregar Matriz AFND
            saida.WriteLine("");
            saida.WriteLine("AUTOMATO NÃO DETERMINISTICO");
            saida.AutoFlush = true;
            for (int linha = 0; linha < (estados.Count + 1); linha++)
            {
                conteudoLinha = "";
                for (int col = 0; col < (tokens.Count + 3); col++)
                {
                    if (linha == 0)
                    {
                        if (col == 0)
                            matriz[linha, col] = "inicial/final";
                        else if (col == 1)
                            matriz[linha, col] = "estados";
                        else if (col-1 > tokens.Count)
                            matriz[linha, col] = "x";
                        else if (col >= 2)
                            matriz[linha, col] = tokens[col - 2];
                    }
                    else
                    {
                        if (col == 1)
                        {
                            if (linha <= estados.Count)
                            {
                                if (estados[linha - 1].inicial)
                                    matriz[linha, col-1] += ">";
                                if (estados[linha - 1].final)
                                    matriz[linha, col-1] += "*";

                                if (matriz[linha, col - 1] == null)
                                    matriz[linha, col - 1] = " ";

                                matriz[linha, col] = estados[linha - 1].nome;
                            }
                        }
                        else if (col > 1)
                        {
                            token = matriz[0, col];
                            estado = matriz[linha, 1];
                            string eOrigem, eDestino;

                            for (int x = 0; x < TabelaAFND.Count; x++)
                            {
                                eOrigem = "";
                                eDestino = "";
                                for (int j = 0; j < TabelaAFND[x].estadoOrigem.Count; j++)
                                    eOrigem += TabelaAFND[x].estadoOrigem[j].nome;

                                if (TabelaAFND[x].token.Equals(token) && eOrigem.Equals(estado))
                                {
                                    for (int h = 0; h < TabelaAFND[x].estadoDestino.Count; h++)
                                        eDestino += TabelaAFND[x].estadoDestino[h].nome +";";

                                    matriz[linha, col] = eDestino;
                                    break;
                                }
                                else
                                    matriz[linha, col] = "x";

                            }
                        }
                    }
                    if (col == 1)
                        conteudoLinha = matriz[linha, col - 1] + ",";
                    conteudoLinha += matriz[linha, col] + ",";
                }
                saida.WriteLine(conteudoLinha);
            }
        }

        public void ImprimirTabelaAFD()
        {
            string estadoDestino = "", estadoOrigem = "";
            //Imprimir Tabela AFND
            for (int i = 0; i < TabelaAFD.Count; i++, estadoOrigem = "", estadoDestino = "")
            {
                for (int x = 0; x < TabelaAFD[i].estadoDestino.Count; x++)
                {
                    if (TabelaAFD[i].estadoDestino.Count == 0)
                        estadoDestino = "-";
                    else
                        estadoDestino = estadoDestino + TabelaAFD[i].estadoDestino[x].nome;
                }
                for (int x = 0; x < TabelaAFD[i].estadoOrigem.Count; x++)
                {
                    if (TabelaAFD[i].estadoOrigem.Count == 0)
                        estadoOrigem = "-";
                    else
                        estadoOrigem = estadoOrigem + TabelaAFD[i].estadoOrigem[x].nome;
                }
                
                Console.WriteLine("| token: " + TabelaAFD[i].token +
                                  "| estadoOrigem: " + estadoOrigem +
                                  "| estadoDestino: " + estadoDestino);
            }

        }

        public void ImprimirTabelaAFND()
        {
            string estadoDestino = "", estadoOrigem = "";
            //Imprimir Tabela AFND
            for (int i = 0; i < TabelaAFND.Count; i++, estadoOrigem = "", estadoDestino = "")
            {
                for (int x = 0; x < TabelaAFND[i].estadoDestino.Count; x++)
                {
                    if (TabelaAFND[i].estadoDestino.Count == 0)
                        estadoDestino = "-";
                    else
                        estadoDestino = estadoDestino + TabelaAFND[i].estadoDestino[x].nome;
                }
                for (int x = 0; x < TabelaAFND[i].estadoOrigem.Count; x++)
                {
                    if (TabelaAFND[i].estadoOrigem.Count == 0)
                        estadoOrigem = "-";
                    else
                        estadoOrigem = estadoOrigem + TabelaAFND[i].estadoOrigem[x].nome;
                }

                Console.WriteLine("| token: " + TabelaAFND[i].token +
                                  "| estadoOrigem: " + estadoOrigem +
                                  "| estadoDestino: " + estadoDestino);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Automato aut = new Automato();
            Console.WriteLine("\nO ARQUIVO DE ENTRADA DEVE ESTAR NA MESMA PASTA DO EXECUTÁVEL.");

            aut.GerarAutomatoNaoDeterministico();        
            aut.DeterminizarAutomato();
            aut.GeraMatrizAFND();
            Console.WriteLine("\nTABELA NAO DETERMINISTICO\n");
            aut.ImprimirTabelaAFND();

            Console.WriteLine("\nTABELA DETERMINISTICO\n");
            aut.GeraMatriz();            
            aut.ImprimirTabelaAFD();

            Console.WriteLine("\nARQUIVO DE SAIDA GERADO NO MESMO LOCAL DO EXECUTÁVEL.\n");

            Console.ReadLine();
        }
    }
}

