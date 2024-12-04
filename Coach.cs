public class Coach
{
    public int NumeroEquipe { get; }
    public string Prenom { get; protected set; } //Prenom imposé si le coach est une IA, au choix sinon
    public bool IA { get; } //true si le coach est une IA, false sinon 
    static private readonly int REANIMER = 30;
    static private readonly int SOIGNER = 30;
    static protected Random rnd = new Random(); //Variable aléatoire qui sert pour les lancer de dés

// --------------------------- CONSTUCTEURS ---------------------------------------------
    public Coach(int numeroEquipe, bool ia)
    {
        NumeroEquipe = numeroEquipe;
        IA = ia;
        Prenom = DemanderPrenom();
    }

    public Coach(int numeroEquipe, string prenom, bool ia)
    {
        NumeroEquipe = numeroEquipe;
        IA = ia;
        Prenom = prenom;
    }

    // --------------------------- PRENOM ---------------------------------------------
    private string DemanderPrenom()
    {

        bool verifie = true;
        string prenom;
        do
        {
            Console.Clear();
            Console.Write($"Equipe {NumeroEquipe} : {new List<string> { "Trolls", "Humains" }[NumeroEquipe - 1]} \nChosissez votre nom d'utilisateur : ");
            prenom = Console.ReadLine()!;
            if (String.IsNullOrEmpty(prenom) == true)
            {
                Console.WriteLine("Erreur :\nLe prénom ne peut être vide.\n");
                verifie = false;
            }
            else
                verifie = true;
        } while (verifie == false);
        return prenom;
    }
    // --------------------------- ACTIONS IA ---------------------------------------------
    public void JouerIA(Plateau plateau)
    {
        // Action de l'IA:
        // Ne peut utiliser qu'un objet par tour
        // Regarde en priorité si un ou des joueurs sont morts
        bool finTour = false; //permet de savoir si l'IA a fini de jouer
        int proba = rnd.Next(1, 101); //proba de jouer son action
        List<int> ordreJoueur = new List<int> { 3, 0, 1, 2 };
        int k = 0;
        string message = "";

        //Si un de ses joueurs est mort et qu'elle a encore une baie magique dans son inventaire, l'IA a 1/3 chance de le réanimer
        //Si plusieurs joueurs sont morts, l'IA soigne en priorité son gardien, puis dans l'ordre joueur 1, 2, 3
        while (k < 4)
        {
            if (((NumeroEquipe == 1 && plateau.Joueurs[k].Mort && plateau.InventaireE1.NbBaieMagique != 0) || (NumeroEquipe == 2 && plateau.Joueurs[k + 3].Mort == true && plateau.InventaireE2.NbBaieMagique != 0)) && proba < REANIMER)
            {
                if (NumeroEquipe == 1)
                    plateau.InventaireE1.UtiliserBaieMagique(plateau.Joueurs[k]);
                else
                    plateau.InventaireE2.UtiliserBaieMagique(plateau.Joueurs[k + 3]);
                message = $"{Prenom} utilise une Baie Magique sur {plateau.Joueurs[k + 3 * (NumeroEquipe - 1)].Prenom}\n{plateau.Joueurs[k + 3 * (NumeroEquipe - 1)].Prenom} est réanimé\n\n";
                finTour = true;
                break;
            }
            k += 1;
        }
        //Si aucun de ses joueurs est mort ou qu'elle n'a plus de baie magique, elle regarde si elle peut soigner des joueurs
        // Soigne seulement si le joueur a perdu au moins 3PV
        //Soigne en priorité celui-ci qui a perdu le plus de PV
        k = 0; //on réinitialise
        int PVPerdu = 2;
        if (((NumeroEquipe == 1 && plateau.InventaireE1.NbBaie != 0) || (NumeroEquipe == 2 && plateau.InventaireE2.NbBaie != 0)) && proba < SOIGNER && finTour == false)
        {
            for (int n = 0; n < 4; n++)
            {
                if (plateau.Joueurs[n + 3 * (NumeroEquipe - 1)].PointdeVieInitial - plateau.Joueurs[n + 3 * (NumeroEquipe - 1)].PointDeVie > PVPerdu && plateau.Joueurs[n + 3 * (NumeroEquipe - 1)].Mort == false)
                {
                    k = n;
                    PVPerdu = plateau.Joueurs[k + 3 * (NumeroEquipe - 1)].PointdeVieInitial - plateau.Joueurs[k + 3 * (NumeroEquipe - 1)].PointDeVie;
                }
            }
            if (PVPerdu > 2)//un des joueurs va être soigné
            {
                if (NumeroEquipe == 1)
                    plateau.InventaireE1.UtiliserBaie(plateau.Joueurs[k], 3);
                else // 2
                    plateau.InventaireE2.UtiliserBaie(plateau.Joueurs[k], 3);
                message = $"{Prenom} utilise une Baie sur {plateau.Joueurs[k + 3 * (NumeroEquipe - 1)].Prenom}\n{plateau.Joueurs[k + 3 * (NumeroEquipe - 1)].Prenom} récupère 3 PV\n\n";
                finTour = true;
            }
        }
        if (finTour == true)
            Console.WriteLine("\n" + message);
        else
            Console.WriteLine($"\n{Prenom} ne fait rien\n");
    }
}