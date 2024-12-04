public class Simulation
{
  static private readonly int POINT_DE_VIE_COUREUR = 10;
  static private readonly int POINT_DE_VIE_DEFENSEUR = 8;
  static private readonly int POINT_DE_VIE_GARDIEN = 3;
  static private readonly int POINT_DE_VIE_BRUTE = 15;
  private FileTools sauvegarde { get; }
  public Plateau PlateauJeu { get; set; }
  public List<Joueur>? Joueurs { get; set; }
  public string TextePourChaqueSimulation { get; private set; }
  public Ballon BallonJeu { get; set; }
  private List<string> Terrains { get; set; }
  public De DeJeu { get; set; }
  public Coach Coach1 { get; protected set; }
  public Coach Coach2 { get; protected set; }

  //---------------------------CONSTRUCTEUR ----------------------------------
  public Simulation()
  {
    sauvegarde = new FileTools();
    DeJeu = new De(); //initialisation du dé
    ParametreJeu(); //introduction au jeu
                    //création des coachs
    List<Coach> choixMode = ChoixMode();
    Coach1 = choixMode[0];
    Coach2 = choixMode[1];
    //choix du terrain
    Terrains = TerrainDeReference();
    List<int> taillePlateau = DemanderTaillePlateau();
    BallonJeu = new Ballon(taillePlateau[0] / 2, taillePlateau[1] / 2);
    //Création des équipes
    List<string> infoJoueurs = new List<string>();
    // List<int> nivGardien = new List<int> { 1, 1 };
    //utilisation du fichier de sauvegarde
    string recupererSauvegarde = ChoixSauvegarde(); //on demande à l'utilisateur s'il veut réutiliser des prénoms sauvegardés
    if (recupererSauvegarde == "2" && sauvegarde.Lire("FichierSauvegarde").Count == 10) //si le fichier est au bon format, on lance la sauvegarde
      infoJoueurs = sauvegarde.Lire("FichierSauvegarde");
    else
    {
      if (recupererSauvegarde == "2")
        Console.WriteLine("La sauvegarde a échouée, veuillez en faire une nouvelle");
      infoJoueurs = CreationEquipes();
    }
    PlateauJeu = new Plateau(BallonJeu, taillePlateau[0], taillePlateau[1]);

    //Equipe 1
    PlateauJeu.AjouterJoueur(new Coureur(infoJoueurs[0], taillePlateau[0] / 2, taillePlateau[1] / 2 - (taillePlateau[0] / 3), POINT_DE_VIE_COUREUR, true));
    PlateauJeu.AjouterJoueur(new Brute(infoJoueurs[1], taillePlateau[0] / 3 - 1, taillePlateau[1] / 2 - 2 * taillePlateau[0] / 3, POINT_DE_VIE_DEFENSEUR, true));
    PlateauJeu.AjouterJoueur(new Defenseur(infoJoueurs[2], 2 * taillePlateau[0] / 3, taillePlateau[1] / 2 - 2 * taillePlateau[0] / 3, POINT_DE_VIE_DEFENSEUR, true));
    PlateauJeu.AjouterJoueur(new Gardien(infoJoueurs[3], taillePlateau[0] / 2, 1, POINT_DE_VIE_GARDIEN, Convert.ToInt32(infoJoueurs[8]), true));
    //Equipe 2
    PlateauJeu.AjouterJoueur(new Coureur(infoJoueurs[4], taillePlateau[0] / 2, taillePlateau[1] / 2 + (taillePlateau[0] / 3), POINT_DE_VIE_COUREUR, false));
    PlateauJeu.AjouterJoueur(new Defenseur(infoJoueurs[5], taillePlateau[0] / 3 - 1, taillePlateau[1] / 2 + 2 * taillePlateau[0] / 3, POINT_DE_VIE_DEFENSEUR, false));
    PlateauJeu.AjouterJoueur(new Brute(infoJoueurs[6], 2 * taillePlateau[0] / 3, taillePlateau[1] / 2 + 2 * taillePlateau[0] / 3, POINT_DE_VIE_DEFENSEUR, false));
    PlateauJeu.AjouterJoueur(new Gardien(infoJoueurs[7], taillePlateau[0] / 2, taillePlateau[1] - 2, POINT_DE_VIE_GARDIEN, Convert.ToInt32(infoJoueurs[9]), false));
    //changement eventuel de classe
    DemandeChangementDeClasse();
    PlateauJeu.NbToursTotal = DemanderNbTour();
    //On initialise le texte à écrire étant vide
    TextePourChaqueSimulation = "";
  }

  //---------------------------INTRODUCTION----------------------------------
  protected int DemanderNbTour()
  {
    bool sol = false;
    string? nbTours;
    do
    {
      Console.WriteLine("Combien de tours de jeu voulez vous simuler ? (min : 1 , max : 100)");

      nbTours = Console.ReadLine()!;
      sol = VerifieSynthaxe(nbTours, 1, 100);
    }
    while (sol == false);
    Console.Clear();
    return Convert.ToInt32(nbTours);
  }

  private void ParametreJeu()
  {
    // Interface du début du jeu.
    // C'est la première interaction avec le joueur.
    // On lui propose de regarder les règles du jeu ou de chosir les paramètres.  
    string reponse1 = "0";
    string reponse2 = "0";
    do
    {
      switch (reponse1)
      {
        case "0":
          Console.Clear();
          Console.WriteLine(Espace(10) + "Bonjour jeune coach en herbe !\n" + Espace(13) + "Bienvenue au Poulpo Bowl !\n");
          Console.WriteLine(Espace(17) + "   _--------_");
          Console.WriteLine(Espace(17) + "  |          |");
          Console.WriteLine(Espace(17) + "  | (O)  (O) |");
          Console.WriteLine(Espace(17) + " _|          |_ ");
          Console.WriteLine(Espace(17) + "|__   ____   __| ");
          Console.WriteLine(Espace(17) + "   |_|    |_|   \n");
          reponse1 = Choix(1, 1, 2);
          break;

        case "1":
          Console.WriteLine("REGLE DU POULPIFOOT\n");
          Console.WriteLine("Vous allez pouvoir assister à une partie de foot très ... particulière !");
          Console.WriteLine("En effet, une bataille affrontant les humains et les trolls est imminente");
          Console.WriteLine("Et quoi de mieux qu’une partie de foot pour régler les conflits !");
          Console.WriteLine("Tous les coups sont permis comme frapper son adversaire pour récupérer la balle.");
          Console.WriteLine("Mais attention, si les points de vie d’un joueur tombent à zéro, celui-ci meurt.\n");
          reponse2 = "0";

          do
          {
            switch (reponse2)
            {
              case "0":
                reponse2 = Choix(12, 0, 2);
                if (reponse2 == "0")
                {
                  reponse2 = "end";
                  reponse1 = "0";
                }
                break;
              case "1":
                InformationsSurLesClasses();
                reponse2 = "0";
                break;
              case "2":
                InformationInventaire();
                reponse2 = "0";
                break;
            }
          }
          while (reponse2 != "end");
          break;
      }
    } while (reponse1 != "2");
  }

  //-----------------------CREATION DES EQUIPES (PRENOM ET CLASSE)--------------------------------
  private string AfficheScore(int longueurTerrain)
  {
    // Cette fonction prend en entrée le score de chaque équipe et l'affiche. 
    //longueurTerrain permet de centrer l'affichage. 
    //retoune le string score
    string score = "";
    string centrer = " ";
    for (int n = 0; n < longueurTerrain / 2 - 5; n++)
      centrer += " ";

    score += centrer + "*** SCORE ***\n" + centrer + " -----------\n" + centrer + "| ";
    for (int i = 0; i < 2; i++)
    {
      if (BallonJeu.Score[i] < 10)
        score += "00" + Convert.ToString(BallonJeu.Score[i]);
      else if (BallonJeu.Score[i] < 100)
        score += "0" + Convert.ToString(BallonJeu.Score[i]);
      else
        score += Convert.ToString(BallonJeu.Score[i]);
      if (i == 0)
        score += " : ";
    }
    score += " |\n" + centrer + " -----------";
    return score;
  }
  private string ChangerPrenomJoueur(int numeroEquipe, int n, List<string> prenomJoueur)
  {
    // même si le joueur vient d'entrer le nom des joueurs, il a encore la possibilité à ce moment là de les changer s'il a fait une erreur
    bool verifie = true;
    string newPrenom = "";
    do
    {
      if (n != 4)
        Console.WriteLine($"Le prénom actuel du joueur n°{n} de l'équipe {numeroEquipe} est {prenomJoueur[n + 4 * (numeroEquipe - 1) - 1]}\n");
      else
        Console.WriteLine($"Le prénom actuel du gardien de l'équipe {numeroEquipe} est {prenomJoueur[n + 4 * (numeroEquipe - 1) - 1]}\n");

      Console.Write("Nouveau prénom : ");
      newPrenom = Console.ReadLine()!;
      if (String.IsNullOrEmpty(newPrenom) == true)
      {
        Console.Clear();
        Console.WriteLine("Erreur :\nLe prénom ne peut être vide.\n");
        verifie = false;
      }
      else
        verifie = true;

    }
    while (verifie == false);
    return newPrenom;
  }

  //-----------------------------CLASSES DES JOUEURS-----------------------
  private void InformationsSurLesClasses()
  {
    //explication des différentes stratégies de jeu des joueurs
    string reponse = "0";
    do
      switch (reponse)
      {
        case "0":
          Console.Clear();
          Console.WriteLine("*** Mes Joueurs ***\n");
          reponse = Choix(13, 0, 4);
          if (reponse == "0")
            reponse = "end";
          break;
        case "1":
          Console.WriteLine("*** Coureur ***\n");
          Console.WriteLine("Le coureur est très utile, c'est lui qui va courir vers le ballon.\nS'il n'a pas le ballon, il court vers lui.");
          Console.WriteLine("Mais garre aux joueurs qui croiseront sa route, il les frappera sans ménagement !\nDès qu'il se retrouve sur la même case que le ballon, il va essayer de le contrôler.");
          Console.WriteLine("(Le coureur n'est pas très habile, cela peut lui prendre un certain temps)\nUne fois la balle en son contrôle, il se dirige vers tout droit vers les buts!");
          Console.WriteLine("Si un joueur lui barre la route, il se déplacera aléatoirement sur une case adjacente.\n");
          Console.WriteLine("Conseil : ayez toujours au moins un coureur dans votre équipe pour aller marquer des buts !\n");
          Console.WriteLine("Appuyer sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;
          reponse = "0";
          break;
        case "2":
          Console.WriteLine("*** Défenseur ***\n");
          Console.WriteLine("Le défenseur n'a qu'un but : évitez que les joueurs adverses possèdent le ballon.");
          Console.WriteLine("Plutôt calme, il ne bougera pas tant qu'un joueur ennemi ne possède pas la balle.");
          Console.WriteLine("Néansmoins dès que cela arrive, il courra vers le possesseur du ballon sans répit!");
          Console.WriteLine("Seul la présence d'un autre joueur sur sa route lui fera dévié son chemin.");
          Console.WriteLine("Et une fois à une case du joueur adverse, il lui piquera le ballon avant de le lancer vers les buts !\n");
          Console.WriteLine("Conseil : Le défenseur est très pratique pour éviter à l'adversaire de venir dans votre camps.");
          Console.WriteLine("          Cependant, il ne vous permettra pas de marquer des buts.\n");
          Console.WriteLine("Appuyer sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;
          reponse = "0";
          break;

        case "3":
          Console.WriteLine("*** Brute ***\n");
          Console.WriteLine("La brute est un peu spéciale, elle ne vient sur le terrain que pour taper les joueurs adverses.");
          Console.WriteLine("Durant la partie, elle se dirigera vers ses adverseires et leur infligera de lourd dégats ");
          Console.WriteLine("Ses coups sont trois fois plus violents que les joueurs normaux ! Ils tuent souvent du premier coup!");
          Console.WriteLine("Néanmoins, la brute est lente elle ne se déplace pas en diagonale.\n");
          Console.WriteLine("Sachez aussi qu'elle n'est pas intéressée par la balle! Quand tout ses adversaires sont morts, elle quitte le terrain !");
          Console.WriteLine("Conseil : La brute est très pratique pour tuer les joueurs adverses.");
          Console.WriteLine("          Cependant, elle ne vous permettra pas de marquer des buts.\n");
          Console.WriteLine("Appuyer sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;
          reponse = "0";
          break;

        case "4":
          Console.WriteLine("*** Gardien ***\n");
          Console.WriteLine("Le gardien est indispensable, chaque équipe en possède un !");
          Console.WriteLine("Il navigue aléatoirement du haut en bas des buts et permet d'arrêter les balles !");
          Console.WriteLine("Attention, les gardiens inexpérimentés (niveau 1) révasse parfois. (Surtout quand ils se trouvent près des buts.)\n");
          //Autre classe idées : tank( déplace lentement mais one shot ennemi), soigneur, mage, poseur de piège,...
          Console.WriteLine("Appuyer sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;
          reponse = "0";
          break;
      }
    while (reponse != "end");
  }
  public void DemandeChangementDeClasse()
  {
    //On demande à l'utilisateur si il veut modifier les classes par défault des joueurs
    AffichageClasseJoueurs(3);
    Console.WriteLine();
    string modifierClasse = Choix(9, 1, 2);
    if (modifierClasse == "1") //Si oui, il modifie la classe de ces joueurs
      ChangerClasse();
  }
  private void AffichageClasseJoueurs(int numeroEquipe)
  { //On affiche la classe des joueurs de l'équipe choisie. 
    //Equipe 1 = trolls // Equipe 2= humains //Equipe 3 = trolls et humains
    Console.Clear();
    if (numeroEquipe == 1 || numeroEquipe == 3)
    {
      Console.WriteLine("Equipe des trolls :");
      foreach (Joueur joueur in PlateauJeu.Joueurs)
      {
        if (joueur.EquipeTroll == true)
          Console.WriteLine($"{joueur.Prenom} est un {joueur.ClasseJoueur()}");
      }
      Console.WriteLine();
    }
    if (numeroEquipe == 2 || numeroEquipe == 3)
    {
      Console.WriteLine("Equipe des humains :");
      foreach (Joueur joueur in PlateauJeu.Joueurs)
      {
        if (joueur.EquipeTroll == false)
          Console.WriteLine($"{joueur.Prenom} est un {joueur.ClasseJoueur()}");
      }
    }
    else if (numeroEquipe != 1)
      Console.WriteLine("Problème, l'argument entré n'est pas valide. Il doit être égal à 1,2 ou 3");
  }
  private void ChangerClasse()
  { //VALABLE POUR DES EQUIPES DE 4
    //Fonction qui demande à l'utilisateur si il veut modifier les classes des joueurs de chaque équipe
    //Il n'est pas possible de modifier la classe du gardien ou d'avoir plusieurs gardien
    int choixJoueur = 0;
    int numeroEquipe;
    string choixClasse = "0";
    do
    {
      AffichageClasseJoueurs(3); //On affiche les joueurs des 2 équipes
      numeroEquipe = Convert.ToInt32(Choix(3, 0, 2)); //On demande de choisir l'équipe
      if (numeroEquipe != 0)
      {//Si le choix est égal à 0, alors on ne fait aucun changement
        do
        {
          Console.Clear();
          AffichageClasseJoueurs(numeroEquipe);
          choixJoueur = Convert.ToInt32(Choix(10, 0, 3));//On demande de choisir un joueur
          if (choixJoueur != 0) //Si on choisit un joueur, on demande maintenant la classe
          {
            if (numeroEquipe == 2) // VALABLE POUR DES EQUIPES DE 4
              choixJoueur += 4;
            Console.WriteLine($"{PlateauJeu.Joueurs[choixJoueur - 1].Prenom} est un {PlateauJeu.Joueurs[choixJoueur - 1].ClasseJoueur()}");
            choixClasse = Choix(11, 0, 3);
            if (choixClasse != "0")
              ChangerClasseUnJoueur(choixJoueur, choixClasse);
          }
        }
        while (choixJoueur != 0);
      }
    }
    while (numeroEquipe != 0); //Si on ne revient pas en arrière à la demande des choix de l'équipe, on nous redemande ce choix   
  }
  private void ChangerClasseUnJoueur(int choixJoueur, string choixClasse)
  { //Fonction qui change la classe d'un joueur avec le numéro du joueur dans la liste joueurs et la choix de la classe
    switch (choixClasse)
    {
      case "1":
        PlateauJeu.Joueurs.Insert(choixJoueur - 1, new Coureur(PlateauJeu.Joueurs[choixJoueur - 1].Prenom, PlateauJeu.Joueurs[choixJoueur - 1].Ligne, PlateauJeu.Joueurs[choixJoueur - 1].Colonne, POINT_DE_VIE_COUREUR, PlateauJeu.Joueurs[choixJoueur - 1].EquipeTroll));
        PlateauJeu.RetirerJoueur(PlateauJeu.Joueurs[choixJoueur]);
        Console.WriteLine($"{PlateauJeu.Joueurs[choixJoueur - 1].Prenom} est maintenant un coureur.");
        break;

      case "2":
        PlateauJeu.Joueurs.Insert(choixJoueur - 1, new Defenseur(PlateauJeu.Joueurs[choixJoueur - 1].Prenom, PlateauJeu.Joueurs[choixJoueur - 1].Ligne, PlateauJeu.Joueurs[choixJoueur - 1].Colonne, POINT_DE_VIE_DEFENSEUR, PlateauJeu.Joueurs[choixJoueur - 1].EquipeTroll));
        PlateauJeu.RetirerJoueur(PlateauJeu.Joueurs[choixJoueur]);
        Console.WriteLine($"{PlateauJeu.Joueurs[choixJoueur - 1].Prenom} est maintenant un défenseur.");
        break;

      case "3":
        PlateauJeu.Joueurs.Insert(choixJoueur - 1, new Brute(PlateauJeu.Joueurs[choixJoueur - 1].Prenom, PlateauJeu.Joueurs[choixJoueur - 1].Ligne, PlateauJeu.Joueurs[choixJoueur - 1].Colonne, POINT_DE_VIE_BRUTE, PlateauJeu.Joueurs[choixJoueur - 1].EquipeTroll));
        PlateauJeu.RetirerJoueur(PlateauJeu.Joueurs[choixJoueur]);
        Console.WriteLine($"{PlateauJeu.Joueurs[choixJoueur - 1].Prenom} est maintenant une brute.");
        break;
    }
  }

  private int DemanderNivGardien()
  {
    return Convert.ToInt32(Choix(19, 1, 3));
  }

  //--------------------------EQUIPES ET PRENOMS---------------------------
  private List<string> CreationEquipes()
  {
    // Le joueur entre le nom de chaque joueur
    Console.Clear();
    string reponse1 = "0";
    string reponse2 = "0";
    string rg = "";
    List<string> prenomJoueurs = new List<string>();
    Console.WriteLine("Sélection de l'équipe 1 : Les Trolls (4 joueurs): \n");
    for (int k = 1; k < 5; k++)
      prenomJoueurs.Add(ChoixJoueur(k));
    string niv1 = (Choix(19, 1, 3));

    Console.Clear();
    Console.WriteLine("Sélection de l'équipe 2 : Les Humains (4 joueurs): \n");
    for (int k = 1; k < 5; k++)
      prenomJoueurs.Add(ChoixJoueur(k));
    prenomJoueurs.Add(niv1);
    prenomJoueurs.Add(Choix(19, 1, 3));

    reponse1 = "0";
    do
    {
      switch (reponse1)
      {
        case "0":
          AfficherEquipe(prenomJoueurs, 2);
          reponse1 = Choix(2, 1, 2);
          break;

        case "2":
          AfficherEquipe(prenomJoueurs, 2);
          reponse2 = Choix(3, 0, 2);
          do
          {
            switch (reponse2)
            {
              case "0":
                reponse1 = "0";
                break;

              default:
                AfficherEquipe(prenomJoueurs, Convert.ToInt32(reponse2) - 1);
                rg = Choix(4, 0, 4);
                if (rg == "0")
                {
                  reponse2 = "0";
                  break;
                }
                prenomJoueurs[Convert.ToInt32(Convert.ToInt32(rg) - 1 + 4 * (Convert.ToInt32(reponse2) - 1))] = ChangerPrenomJoueur(Convert.ToInt32(reponse2), Convert.ToInt32(rg), prenomJoueurs);

                break;
            }
          }
          while (reponse1 != "0");
          break;
      }
    }
    while (reponse1 != "1");

    //On demande à l'utilisateur si il veut sauvegarder le nom des joueurs
    string vouloirSave = Choix(18, 1, 2);
    if (vouloirSave == "1")
    {
      string prenoms = "";
      foreach (string prenom in prenomJoueurs)
      {
        prenoms += prenom;
        prenoms += "\r\n";
      }
      sauvegarde.Ecrire("FichierSauvegarde", prenoms);
    }

    return prenomJoueurs;
  }
  private void AfficherEquipe(List<string> prenomJoueurs, int n)
  {
    //affichage global des équipes (nom des joueurs)
    Console.Clear();
    switch (n)
    {
      case 2:
        Console.WriteLine("Membres de l'équipe 1" + Espace(20) + "Membres de l'équipe 2\n");
        for (int k = 1; k < 4; k++)
        {
          Console.WriteLine($"- Joueur n°{k} : {prenomJoueurs[k - 1]}" + Espace(26 - prenomJoueurs[k - 1].Length) + $"- Joueur n°{k} : {prenomJoueurs[k + 3]}");
        }
        Console.WriteLine($"- Gardien : {prenomJoueurs[3]}" + Espace(29 - prenomJoueurs[3].Length) + $"- Gardien : {prenomJoueurs[7]}");
        break;

      case < 2: // que 0 (=équipe 1) ou 1 (=équipe 2)
        Console.WriteLine("Membres de l'équipe 1\n");
        for (int k = 1; k < 4; k++)
        {
          Console.WriteLine($"- Joueur n°{k} : {prenomJoueurs[k - 1 + 4 * n]}");
        }
        Console.WriteLine($"- Gardien : {prenomJoueurs[3 + 4 * n]}");
        break;
    }
  }
  private string ChoixJoueur(int numeroJoueur)
  {
    //le joueur entre le nom de chaque joueur
    bool verifie = true;
    string prenom = " ";
    do
    {
      if (numeroJoueur == 4) //le gardien
        Console.Write("Choisissez le prénom du gardien : ");
      else
        Console.Write($"Choisissez le prénom du joueur n°{numeroJoueur} : ");
      prenom = Console.ReadLine()!;

      if (String.IsNullOrEmpty(prenom) == true)
      {
        Console.WriteLine("Erreur :\nLe prénom ne peut être vide.\n");
        verifie = false;
      }
      else
        verifie = true;
    }
    while (verifie == false);

    return prenom;
  }
  public string ChoixSauvegarde()
  {  //Demande à l'utilisateur s'il veut utiliser une sauvegarde des prénoms
    string reponse;
    bool sol;
    Console.Clear();
    do
    {
      Console.WriteLine("Choisissez les prénoms des joueurs :\n");

      Console.WriteLine(" ---------------------------------            --------------------------------------- ");
      Console.WriteLine("| Choisir de nouveaux prénoms : 1 |          | Reprendre les prénoms sauvegardés : 2 |");
      Console.WriteLine(" ---------------------------------            --------------------------------------- ");
      reponse = Console.ReadLine()!;
      sol = VerifieSynthaxe(reponse, 1, 2);
    }
    while (sol == false);
    Console.Clear();
    return reponse;
  }

  //-----------------------AFFICHAGE PLATEAU-------------------------------

  private List<string> TerrainDeReference()
  {
    // Permet de présenter, lors du choix du terrain, à quoi ressemblera celui-ci lors de la partie
    //afin que le joueur puisse selectionner définitivement ou non ce terrain

    //Petit terrain
    string petitTerrain = " -------------------------\n | - - - - - - - - - - - |\n";
    for (int i = 0; i < 2; i++)
      petitTerrain += " | :         :         : |\n";
    for (int i = 0; i < 3; i++)
      petitTerrain += "╬╬           :           ╬╬\n";
    for (int i = 0; i < 2; i++)
      petitTerrain += " | :         :         : |\n";
    petitTerrain += " | - - - - - - - - - - - |\n -------------------------\n";

    //terrainMoyen
    string terrainMoyen = " -----------------------------------------\n | - - - - - - - - - - - - - - - - - - - |\n";
    for (int i = 0; i < 3; i++)
      terrainMoyen += " | :                 :                 : |\n";
    for (int i = 0; i < 5; i++)
      terrainMoyen += "╬╬                   :                   ╬╬\n";
    for (int i = 0; i < 3; i++)
      terrainMoyen += " | :                 :                 : |\n";
    terrainMoyen += " | - - - - - - - - - - - - - - - - - - - |\n -----------------------------------------\n";

    //grand terrain
    string grandTerrain = " ---------------------------------------------------------\n | - - - - - - - - - - - - - - - - - - - - - - - - - - - |\n";
    for (int i = 0; i < 6; i++)
      grandTerrain += " | :                         :                         : |\n";
    for (int i = 0; i < 7; i++)
      grandTerrain += "╬╬                           :                           ╬╬\n";
    for (int i = 0; i < 6; i++)
      grandTerrain += " | :                         :                         : |\n";
    grandTerrain += " | - - - - - - - - - - - - - - - - - - - - - - - - - - - |\n ---------------------------------------------------------\n";

    return new List<string> { petitTerrain, terrainMoyen, grandTerrain };
  }
  private List<int> DemanderTaillePlateau()
  {
    //on demande au joueur de choisir la taille du terrain avec lequel il va jouer
    string reponse = "0";
    string reponse2 = "0";
    Console.WriteLine("Choix du terrain ...");
    // Plateau petitPlateau = new Plateau(new Ballon(9 / 2, 23 / 2), 9, 23);
    // Plateau plateauMoyen = new Plateau(new Ballon(15 / 2, 39 / 2), 15, 39);
    // Plateau grandPlateau = new Plateau(new Ballon(21 / 2, 55 / 2), 21, 55);
    bool verifie = true;

    while (reponse2 == "0")
    {
      if (reponse == "0")
      {
        Console.Clear();
        do
        {

          Console.WriteLine("Quel taille de terrain voulez vous ?\n");
          Console.WriteLine(" -------------------       ---------------------       -------------------");
          Console.WriteLine("| Petit terrain = 1 |     | Terrain moyenne = 2 |     | Grand terrain = 3 |");
          Console.WriteLine(" -------------------       ---------------------       -------------------");
          Console.Write("\nChoix : ");
          reponse = Console.ReadLine()!;
          verifie = VerifieSynthaxe(reponse, 1, 3);
        }
        while (verifie == false);

      }

      else
      {
        Console.Clear();
        if (reponse == "1")
        {
          Console.WriteLine("Petit terrain : \n");
          Console.WriteLine(Terrains[0]);
        }

        else if (reponse == "2")
        {
          Console.WriteLine("Terrain moyen : \n");
          Console.WriteLine(Terrains[1]);
        }
        else //reponse =="3"
        {
          Console.WriteLine("Grand terrain: \n");
          Console.WriteLine(Terrains[2]);
        }

        do
        {
          Console.WriteLine("\n ------------------       ------------");
          Console.WriteLine("| Sélectionner = 1 |     | Retour = 0 |");
          Console.WriteLine(" ------------------       ------------");
          Console.Write("\nChoix : ");
          reponse2 = Console.ReadLine()!;
          verifie = VerifieSynthaxe(reponse2, 0, 1);
        }
        while (verifie == false);

        if (reponse2 == "0")
          reponse = "0";
      }
    }
    if (reponse == "1")
      return new List<int> { 9, 23 };
    else if (reponse == "2")
      return new List<int> { 15, 39 };
    else //reponse=="3"
      return new List<int> { 21, 55 };
  }

  //-----------------INTERACTION UTILISATEUR ET AUTRES AFFICHAGES-------------
  private string Choix(int n, int a, int b)
  {
    // Cette fonction permet au joueur de prendre des décisions durant le parametrage du jeu et pendant une partie
    // n (int) permet de savoir quelles réponses proposer
    // a et b (int) sont les différents chiffres à taper possible pour répondre 
    string reponse;
    bool sol;
    do
    {
      switch (n)
      {
        case 1: // on demande au joueur s'il veut voir les règles du jeu ou commencer à régler les paramètres (utilisé dans ParametreJeu)
          Console.WriteLine(" ------------------            --------------------------");
          Console.WriteLine("| Règle du jeu : 1 |          | Choix des paramètres : 2 |");
          Console.WriteLine(" ------------------            -------------------------- ");
          break;
        case 2: // on demande au joueur de choisir (utilisé dans CreationEquipes)
          Console.WriteLine("\n -----------------------                 --------------");
          Console.WriteLine("| Commencer à jouer = 1 |               | Modifier = 2 |");
          Console.WriteLine(" -----------------------                 --------------");
          break;
        case 3: // on demande au joueur de choisir quelle équipe il souhaite modifier (utilisé dans CreationEquipes)
          Console.WriteLine("\nQuelle équipe voulez-vous modifier ?\n");
          Console.WriteLine(" --------------           --------------           ------------");
          Console.WriteLine("| Equipe 1 = 1 |         | Equipe 2 = 2 |         | Retour = 0 |");
          Console.WriteLine(" --------------           --------------           ------------");
          break;
        case 4: // on demande au joueur de choisir quel prénom il souhaite modifier (utilisé dans CreationEquipes)

          Console.WriteLine("\nQuel prénom voulez-vous modifier ?");
          Console.WriteLine(" ----------------   ----------------   ----------------   -------------   --------------");
          Console.WriteLine("| Joueur n°1 = 1 | | Joueur n°2 = 2 | | Joueur n°3 = 3 | | Gardien = 4 | | Terminer = 0 |");
          Console.WriteLine(" ----------------   ----------------   ----------------   -------------   --------------");
          break;
        case 5: //on demande au joueur s'il veut soigner un membre de son équipe
          Console.WriteLine("\n ---------------   -------------");
          Console.WriteLine("| Continuer = 1 | | Soigner = 2 |");
          Console.WriteLine(" ---------------   -------------");
          break;
        case 6: //on demande au joueur ce qu'il veut utiliser (si équipe 1)
          Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
          Console.WriteLine(PlateauJeu);
          Console.WriteLine("Que voulez utiliser ?");
          Console.WriteLine(" --------------   -----------------------   ------------");
          Console.WriteLine($"| Baie ({PlateauJeu.InventaireE1.NbBaie}) = 1 | | Baie Magique ({PlateauJeu.InventaireE1.NbBaieMagique}) = 2 | | Retour = 0 |");
          Console.WriteLine(" --------------   -----------------------   ------------");
          break;
        case 7: //on demande au joueur ce qu'il veut utiliser (si équipe 2)
          Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
          Console.WriteLine(PlateauJeu);
          Console.WriteLine("Que voulez utiliser ?");
          Console.WriteLine(" --------------   -----------------------   ------------");
          Console.WriteLine($"| Baie ({PlateauJeu.InventaireE2.NbBaie}) = 1 | | Baie Magique ({PlateauJeu.InventaireE2.NbBaieMagique}) = 2 | | Retour = 0 |");
          Console.WriteLine(" --------------   -----------------------   ------------");
          break;
        case 8: //demande au coach quel joueur de son équipe il veut soigner
          Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
          Console.WriteLine(PlateauJeu);
          Console.WriteLine("\nQui voulez-vous soigner ?");
          Console.WriteLine(" ----------------   ----------------   ----------------   -------------   ------------ ");
          Console.WriteLine("| Joueur n°1 = 1 | | Joueur n°2 = 2 | | Joueur n°3 = 3 | | Gardien = 4 | | Retour = 0 |");
          Console.WriteLine(" ----------------   ----------------   ----------------   -------------   ------------ ");
          break;
        case 9: //on demande confirmation au coach s'il veut modifier la classe d'un de ses joueurs
          Console.WriteLine("Souhaitez vous modifier la classe d'un des joueur(s) ?\n");
          Console.WriteLine(" ---------    --------- ");
          Console.WriteLine("| Oui = 1 |  | Non = 2 |");
          Console.WriteLine(" ---------    --------- ");
          break;
        case 10: // on demande au coach quel joueur il veut selectionner pour modifier se classe
          Console.WriteLine("\nDe quel joueur voulez-vous modifier la classe ?");
          Console.WriteLine(" ----------------   ----------------   ----------------    --------------");
          Console.WriteLine("| Joueur n°1 = 1 | | Joueur n°2 = 2 | | Joueur n°3 = 3 |  | Terminer = 0 |");
          Console.WriteLine(" ----------------   ----------------   ----------------    --------------");
          break;
        case 11://on demande u joueur de choisir la classe qu'il veut pour son joueur
          Console.WriteLine("\nQuelle classe voulez-vous sélectionner ?");
          Console.WriteLine(" -------------    ---------------    -----------    -------------");
          Console.WriteLine("| Coureur = 1 |  | Défenseur = 2 |  | Brute = 3 |  | Annuler = 0 |");
          Console.WriteLine(" -------------    ---------------    -----------    -------------");
          break;
        case 12: //intervient dans ParametreJeu dans la lectures des règles du jeu
          Console.WriteLine(" -----------------   --------------------   ------------");
          Console.WriteLine("| Mes Joueurs = 1 | | Mon inventaire = 2 | | Retour = 0 |");
          Console.WriteLine(" -----------------   --------------------   ------------");
          break;
        case 13: //intervient dans ParametreJeu dans la lectures des règles du jeu
          Console.WriteLine(" -------------   ---------------   -------------   -------------   ------------ ");
          Console.WriteLine("| Coureur = 1 | | Défenseur = 2 | |  Brute = 3  | | Gardien = 4 | | Retour = 0 |");
          Console.WriteLine(" -------------   ---------------   -------------   -------------   ------------ ");
          break;
        case 14:  //intervient dans ParametreJeu dans la lectures des règles du jeu
          Console.WriteLine(" ----------   ------------------   ------------");
          Console.WriteLine("| Baie = 1 | | Baie Magique = 2 | | Retour = 0 |");
          Console.WriteLine(" ----------   ------------------   ------------");
          break;
        case 15: // on demande au coach avec quel mode de jeu il veut jouer
          Console.WriteLine("Choisissez un mode de jeu\n");
          Console.WriteLine(" ---------------   --------------------   ----------------------");
          Console.WriteLine("| Mode Solo = 1 | | Mode 2 Joueurs = 2 | | Mode Automatique = 3 |");
          Console.WriteLine(" ---------------   --------------------   ----------------------");
          break;
        case 16: //on demande au coach quelle équipe il veut soutenir
          Console.WriteLine("Quelle équipe voulez-vous soutenir ?\n");
          Console.WriteLine(" -----------------------   ------------------------   ------------");
          Console.WriteLine("| Equipe des Trolls = 1 | | Equipe des Humains = 2 | | Retour = 0 |");
          Console.WriteLine(" -----------------------   ------------------------   ------------");
          break;
        case 17:
          Console.WriteLine(" ----------------   ------------");
          Console.WriteLine("| Continuer  = 1 | | Retour = 0 |");
          Console.WriteLine(" ----------------   ------------");
          break;
        case 18:
          Console.WriteLine(" Voulez vous sauvegarder les prénoms des joueurs ?\n");
          Console.WriteLine(" ----------   ---------");
          Console.WriteLine("| Oui  = 1 | | Non = 2 |");
          Console.WriteLine(" ----------   ---------");
          break;
        case 19:
          Console.WriteLine("\nChoisissez le niveau du gardien \n");
          Console.WriteLine(" --------------   --------------   --------------");
          Console.WriteLine("| Niveau 1 = 1 | | Niveau 2 = 2 | | Niveau 3 = 3 |");
          Console.WriteLine(" --------------   --------------   --------------");
          break;
      }

      Console.Write("\nChoix : ");
      reponse = Console.ReadLine()!;
      sol = VerifieSynthaxe(reponse, a, b);
    }
    while (sol == false);
    Console.Clear();
    return reponse;
  }
  private string Espace(int n)
  {
    string espace = "";
    for (int i = 0; i < n; i++)
      espace += " ";
    return espace;
  }
  private bool VerifieSynthaxe(string reponse, int a, int b) // la même que le projet memento mori
  {
    //Cette fonction vérifie si la réponse entrée par le joueur est valide.
    //-elle retourne true si la reponse est valide
    //-sinon, elle retourne false
    //
    //Si l'on veut juste vérifier que la réponse est bien un (int):
    // - a=0 et b=0
    //--> la fonction vérifie si la réponse est bien un (int)
    //
    //Si l'on veut vérifier que la réponse est bien dans un intervalle [|a,b|]:
    // - a!=0 et b!=0
    //--> la fonction vérifie d'abord si la réponse est bien un (int)
    //--> elle vérifie ensuite si cette réponse est bien dans l'intervalle [|a,b|]
    //
    // reponse (string) est la réponse entrée par l'utilisateur 
    // a (int) et b (int) sont les entiers de l'intervalle de restriction de valeurs [|a,b|]

    bool sol = false;
    int reponseInt;
    //Création de l'intervalle
    int[] valeurAutorise = new int[b - a + 1];
    for (int k = 0; k < b - a + 1; k++)
    {
      valeurAutorise[k] = a + k;
    }

    //On vérifie si reponse est bien un (int)
    if (int.TryParse(reponse, out reponseInt) == false)
    {
      Console.Clear();
      Console.WriteLine("\nErreur :\nVotre réponse n'est pas valide, vous devez tapez un nombre.\n");
    }

    //Si il n'y a pas d'intervalle de restriction de valeur, alors la réponse est autorisée
    else if (a == 0 && b == 0)
    {
      sol = true;
    }

    //Si l'intervalle est restreint, on vérifie que reponse est inclu dans l'intervalle
    else
    {
      reponseInt = Convert.ToInt32(reponse);
      for (int k = 0; k < valeurAutorise.Length; k++)
      {
        if (valeurAutorise[k] == reponseInt)
        {
          sol = true;
          break;
        }
      }
      if (sol == false)
      {
        Console.Clear();
        Console.WriteLine("\nErreur :\nVotre réponse n'est pas valide, vous devez tapez un des chiffre parmis ceux proposés.\n");
      }
    }
    return sol;
  }

  //------------------------GESTION DE L'INVENTAIRE---------------------------------
  private void InformationInventaire()
  {
    //information sur les objets de l'inventaire, intervient dans les règles du jeux
    string reponse = "0";
    do
    {
      switch (reponse)
      {
        case "0":
          Console.Clear();
          Console.WriteLine("*** Mon inventaire ***\n");
          Console.WriteLine("A chaque tour, vous aurez la possibilité d'utiliser des objets de votre inventaires\n");
          reponse = Choix(14, 0, 2);
          if (reponse == "0")
            reponse = "end";
          Console.Clear();
          break;
        case "1":

          Console.WriteLine("*** Baie ***\n");
          Console.WriteLine("Les baies vous permettrons de redonner 3PV à un de vos joueurs.\n");
          Console.WriteLine("Appuyez sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;
          reponse = "0";
          break;
        case "2":
          Console.WriteLine("*** Baie Magique ***\n");
          Console.WriteLine("Les baies magiques permettent de réanimer un de vos joueurs. \nIl sera alors en pleine forme et récupèrera tous ses PV\n");
          Console.WriteLine("Appuyez sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;
          reponse = "0";
          break;
      }
    } while (reponse != "end");
  }
  private void Soigner(int equipe)
  {
    //permet au coach de soigner une de ses joueurs
    bool verifie = true;
    string reponse = "-1";
    string choixJoueur;

    do
    {
      switch (reponse)
      {
        case "-1":
          do
          {
            reponse = Choix(5 + equipe, 0, 2); //on demande ce que veut utiliser le joueur, choix 6 si equipe==1, choix7 si equipe==2
            if ((reponse == "1" && ((equipe == 1 && PlateauJeu.InventaireE1.NbBaie == 0) || (equipe == 2 && PlateauJeu.InventaireE2.NbBaie == 0))) || (reponse == "2" && (equipe == 1 && PlateauJeu.InventaireE1.NbBaieMagique == 0) || (equipe == 2 && PlateauJeu.InventaireE2.NbBaieMagique == 0)))
            {
              Console.WriteLine("Vous n'avez plus de baie !");
              verifie = false;
            }
            else
              verifie = true;
          }
          while (verifie == false);
          break;

        case "1": // le joueur veut utiliser une baie
          choixJoueur = Choix(8, 0, 4);
          if (choixJoueur != "0")
          {
            if (PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)].PointDeVie == (PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)].PointdeVieInitial))
            {
              Console.WriteLine("Vous ne pouvez pas utiliser de baie sur ce joueur"); //le joueur a déjà ses PV au max
              verifie = false;
            }
            else
            {
              if (equipe == 1)
                PlateauJeu.InventaireE1.UtiliserBaie(PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)], 3);
              else // equipe==2
                PlateauJeu.InventaireE2.UtiliserBaie(PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)], 3);
            }

          }
          reponse = "-1";
          break;

        case "2":// le joueur veut utiliser une baie magique
          choixJoueur = Choix(8, 0, 4);
          if (choixJoueur != "0")
          {
            if (PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)].PointDeVie != 0)
            {
              Console.WriteLine("Vous ne pouvez pas utiliser de baie magique sur ce joueur"); //le joueur n'est pas mort
              verifie = false;
            }
            else
            {
              if (equipe == 1)
                PlateauJeu.InventaireE1.UtiliserBaieMagique(PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)]);
              else // equipe==2
                PlateauJeu.InventaireE2.UtiliserBaieMagique(PlateauJeu.Joueurs[Convert.ToInt32(choixJoueur) + 1 * (equipe - 2) + 3 * (equipe - 1)]);
            }
          }
          reponse = "-1";
          break;
      }
    }
    while (reponse != "0");
  }
  //--------------------------MODE DE JEU-----------------------
  private List<Coach> ChoixMode()
  {
    //le joueur choisit avec quelle mode de jeu il veut jouer
    List<Coach> coachs = new List<Coach>();
    string mode = "0";
    string reponse2;
    do
    {
      switch (mode)
      {
        case "0":
          mode = Choix(15, 1, 3);
          break;
        case "1":
          Console.WriteLine("*** Mode Solo ***\n");
          Console.WriteLine("Dans ce mode de jeu, vous allez pouvoir soutenir une des équipes de votre choix.\nVous défirez alors une IA \n");
          reponse2 = Choix(16, 0, 2);
          switch (Convert.ToInt32(reponse2))
          {
            case 0:
              mode = "0";
              break;
            case 1:
              coachs.Add(new Coach(Convert.ToInt32(reponse2), false));
              coachs.Add(new Coach(2, "Patate", true));
              mode = "end";
              break;
            case 2:
              coachs.Add(new Coach(1, "Le Roi des Trolls", true));
              coachs.Add(new Coach(2, false));
              mode = "end";
              break;
          }
          break;

        case "2":
          Console.WriteLine("*** Mode 2 Joueurs ***\n");
          Console.WriteLine("Affrontez vous en duel avec vos amis !\n");
          reponse2 = Choix(17, 0, 1);
          if (reponse2 == "0")
            mode = "0";
          else
          {
            coachs.Add(new Coach(1, false));
            coachs.Add(new Coach(2, false));
            mode = "end";
          }
          break;
        case "3":
          Console.WriteLine("** Mode 2 Joueurs ***\n");
          Console.WriteLine("Regardez le duel entre Le roi des Trolls et Patate !");
          coachs.Add(new Coach(1, "Le Roi des Trolls", true));
          coachs.Add(new Coach(2, "Patate", true));
          mode = "end";
          break;
      }
    }
    while (mode != "end");
    return coachs;
  }

  // -------------------------SIMULATION---------------------------------------
  private List<int> FinJeuParMort()
  {
    List<int> nbJoueurVivant = new List<int> { 0, 0 };
    //on verifie si la partie est termine

    for (int k = 0; k < 4; k++)
      if (PlateauJeu.Joueurs[k].Mort == false)
        nbJoueurVivant[0] += 1;
    for (int k = 4; k < 8; k++)
      if (PlateauJeu.Joueurs[k].Mort == false)
        nbJoueurVivant[1] += 1;
    return nbJoueurVivant;
  }
  private void AffichageFinJeu(int equipeGagnante)
  {
    //affichage de fin de jeu 
    // si equipeGagnante = 1 ou 2 on affiche un message de victoire/défaite
    // si equipeGagnante = 0 c'est une égalité
    Console.Clear();
    switch (equipeGagnante)
    {
      case 0:
        Console.WriteLine("Egalité !\n");
        Console.WriteLine(Espace(17) + "   _--------_");
        Console.WriteLine(Espace(17) + "  |          |");
        Console.WriteLine(Espace(17) + "  | (O)  (O) |");
        Console.WriteLine(Espace(17) + " _|          |_ ");
        Console.WriteLine(Espace(17) + "|__   ____   __| ");
        Console.WriteLine(Espace(17) + "   |_|    |_|   \n");
        if (Coach1.IA == false) //si le coach (ou l'un des coachs en mode 2 joueurs) soutenait l'équipe 1
          Console.WriteLine($"Beau match {Coach1.Prenom}, mais nous pouvons mieux faire.\nCe petit poulpe ne nous échappera pas !");
        if (Coach2.IA == false) //si le coach (ou l'un des coachs en mode 2 joueurs) soutenait l'équipe 2
          Console.WriteLine($"Beau match {Coach2.Prenom}, mais la guerre n'est pas terminée.\nContinuons de protéger Poulpi comme il se doit !");
        break;

      case 1:
        Console.WriteLine("L'équipe 1 a gagné !\nGloire aux Trolls !\n");
        Console.WriteLine(Espace(17) + "   _--------_");
        Console.WriteLine(Espace(17) + "  |          |");
        Console.WriteLine(Espace(17) + "  | (X)  (X) |");
        Console.WriteLine(Espace(17) + " _|          |_ ");
        Console.WriteLine(Espace(17) + "|__   ____   __| ");
        Console.WriteLine(Espace(17) + "   |_|    |_|   \n");
        if (Coach1.IA == false) //si le coach (ou l'un des coachs en mode 2 joueurs) soutenait l'équipe 1
          Console.WriteLine($"Miam maim, ce petit poulpe à l'air délicieux...\nBravo {Coach1.Prenom}, nous allons nous régaler !");
        if (Coach2.IA == false) //si le coach (ou l'un des coachs en mode 2 joueurs) soutenait l'équipe 2
          Console.WriteLine($"Oh non ! Poulpi va être dévoré par les trolls !\n{Coach2.Prenom} il est encore temps de le sauver, rejouons !");
        break;

      case 2:
        Console.WriteLine("L'équipe 2 a gagné !\nGloire à l'ENSC !\n");
        Console.WriteLine(Espace(17) + "   _--------_");
        Console.WriteLine(Espace(17) + "  |          |");
        Console.WriteLine(Espace(17) + "  | (^)  (^) |");
        Console.WriteLine(Espace(17) + " _|          |_ ");
        Console.WriteLine(Espace(17) + "|__   ____   __| ");
        Console.WriteLine(Espace(17) + "   |_|    |_|   \n");
        if (Coach1.IA == false) //si le coach (ou l'un des coachs en mode 2 joueurs) soutenait l'équipe 1
          Console.WriteLine($"Oh non ! Ces délicieux poulpes ... \nNe vous inquiétez pas {Coach1.Prenom}, la prochaine fois, nous gagnerons !");
        if (Coach2.IA == false) //si le coach (ou l'un des coachs en mode 2 joueurs) soutenait l'équipe 2
          Console.WriteLine($"Bravo {Coach2.Prenom}, vous avez sauvé Poulpi !");
        break;
    }
  }
  private int DetermineGagnant()
  {
    List<int> nbJoueurVivant = FinJeuParMort();
    //on regarde le score des équipes pour déterminer le gagnant
    //si le score est a égalité, on regarde le nb de joueur mort dans chaque équipe
    if (PlateauJeu.BallonPlateau.Score[0] > PlateauJeu.BallonPlateau.Score[1])
      return 1;
    else if (PlateauJeu.BallonPlateau.Score[0] < PlateauJeu.BallonPlateau.Score[1])
      return 2;
    else //égalité au niv des scores, on va regarder le nb de joueur vivant dans chaque équipe
    {
      //on compte le nb de joueurs encore vivant
      if (nbJoueurVivant[0] > nbJoueurVivant[1])
        return 1;
      else if (nbJoueurVivant[0] < nbJoueurVivant[1])
        return 2;
      else
        return 0;
    }
  }
  public void Simuler()
  {
    string reponse;

    //premier affichage
    Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
    Console.WriteLine(PlateauJeu);
    Console.WriteLine($"Le match affrontant l'équipe des Trolls dirigé par {Coach1.Prenom} et\nl'équipe des Etudiants de l'ENSC coaché par {Coach2.Prenom} est sur le point de commencer !\n\n");
    Console.Write("Appuyer sur 'Entrée' pour déterminer quelle équipe commence. ");
    reponse = Console.ReadLine()!;

    //on lance le premier dé pour déterminer quelle équipe commence
    Console.WriteLine(DeJeu);
    Console.WriteLine("\nAppuyer sur 'Entrée' pour continuer. ");
    reponse = Console.ReadLine()!;
    //en fonction du résultat du dé, on change la position du ballon
    switch (DeJeu.ValeurDe)
    {
      case 1:
        BallonJeu.RemiseAZero(PlateauJeu, -1);
        break;
      case 2:
        BallonJeu.RemiseAZero(PlateauJeu, 1);
        break;
    }
    PlateauJeu.NettoyagePlateauJeu();

    //Interface de jeu juste avant le début de la partie, le joueur appuie sur Entrée lorsqu'il est pret à démarrer
    Console.Clear();
    Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
    Console.WriteLine(PlateauJeu);
    Console.Write("Appuyer sur 'Entrée' pour commnencer la partie");
    reponse = Console.ReadLine()!;
    while (PlateauJeu.NbTours != PlateauJeu.NbToursTotal)
    {
      UnTourDeJeu(reponse);
      if (FinJeuParMort()[0] <= 1 || FinJeuParMort()[1] <= 1)//si tous les joueurs d'une équipe sont morts (hors gardien) on arrete la partie
        break;
      PlateauJeu.NbTours += 1;
    }
    Console.WriteLine("La partie est terminée !");
    if (FinJeuParMort()[0] <= 1 || FinJeuParMort()[1] <= 1) //on fait de cette façon pour bien afficher dans le cas où les 2 équipe seraient à terre
    {
      if (FinJeuParMort()[0] <= 1)
        Console.WriteLine($"L'équipe 1 est a terre");
      if (FinJeuParMort()[1] <= 1)
        Console.WriteLine($"L'équipe 2 est a terre");
    }
    else
      Console.WriteLine("Il n'y a plus de tours");
    Console.WriteLine("\nAppuyez sur 'Entrée' pour continuer");
    reponse = Console.ReadLine()!;
    AffichageFinJeu(DetermineGagnant());
  }
  public void UnTourDeJeu(string reponse)
  {
    TextePourChaqueSimulation = ""; //A chaque tour, on réinitialise à zéro le texte à afficher
    PlateauJeu.NettoyagePlateauJeu();
    Console.Clear();

    if (Coach1.IA != true || Coach2.IA != true) //Si les 2 joueurs ne sont pas des IA, on fait demande à l'utilisateur d'appuyer sur entrée
    {
      foreach (Joueur joueur in PlateauJeu.Joueurs)
      {
        if (joueur.Mort == false)
          joueur.Deplacer(PlateauJeu, BallonJeu); //on fait jouer chaque joueur (si ils ne sont pas mort)
        TextePourChaqueSimulation += joueur.TexteJoueur; //On récupère le texte de l'action de chaque joueur
        joueur.ReinitialiserTexteJoueur(); //On réinitialise le texte du joueur pour le prochain tour
      }
      BallonJeu.DeplacerBallon(PlateauJeu);

      for (int equipe = 1; equipe < 3; equipe++) //on fait jouer les coachs
      {
        Console.Clear();
        Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
        Console.WriteLine(PlateauJeu);
        Console.WriteLine(TextePourChaqueSimulation);
        Console.WriteLine(PlateauJeu.MortDurantLeTour());
        Console.Write($"----------------------------------------------------\nC'est au tour de l'équipe {equipe}. ");
        if (equipe == 1)
          Console.WriteLine($"{Coach1.Prenom} à toi de jouer !");
        else
          Console.WriteLine($"{Coach2.Prenom} à toi de jouer !");
        //si le coach de l'équipe n'est pas une IA, on demande à l'utilisateur de jouer
        //sinon on fait jouer l'IA
        if ((equipe == 1 && Coach1.IA == false) || (equipe == 2 && Coach2.IA == false))
        {
          reponse = Choix(5, 1, 3);
          if (reponse != "1")
            Soigner(equipe);
        }
        else
        {
          if (equipe == 1)
          {
            Coach1.JouerIA(PlateauJeu);
          }

          else
            Coach2.JouerIA(PlateauJeu);
          Console.WriteLine("Appuyez sur 'Entrée' pour continuer");
          reponse = Console.ReadLine()!;

        }
      }
    }
    else
    { //Les deux coachs sont des IA

      TextePourChaqueSimulation = ""; //A chaque tour, on réinitialise à zéro le texte à afficher
      PlateauJeu.NettoyagePlateauJeu();
      Console.Clear();
      foreach (Joueur joueur in PlateauJeu.Joueurs) //Tour de jeu de l'équipe troll
      {
        if (joueur.Mort == false)
          joueur.Deplacer(PlateauJeu, BallonJeu); //on fait jouer chaque joueur (si ils ne sont pas mort)
        TextePourChaqueSimulation += joueur.TexteJoueur; //On récupère le texte de l'action de chaque joueur
        joueur.ReinitialiserTexteJoueur(); //On réinitialise le texte du joueur pour le prochain tour
      }
      BallonJeu.DeplacerBallon(PlateauJeu);

      //Affichage du plateau et du score
      Console.WriteLine(AfficheScore(PlateauJeu.Colonne));
      Console.WriteLine(PlateauJeu);
      Console.WriteLine(TextePourChaqueSimulation);

      //Affichage du texte des IA
      if (TextePourChaqueSimulation != "")
      {
        Thread.Sleep(1000);
        Console.WriteLine($"----------------------------------------------------");
      }

      Console.Write($"Equipe des trolls : {Coach1.Prenom} à toi de jouer !");
      Thread.Sleep(1000);
      Coach1.JouerIA(PlateauJeu);

      Thread.Sleep(1000);
      Console.WriteLine($"----------------------------------------------------");

      Console.Write($"Equipe des étudiants de l'ENSC : {Coach2.Prenom} à toi de jouer !");
      Thread.Sleep(1000);
      Coach2.JouerIA(PlateauJeu);

      Console.WriteLine($"----------------------------------------------------");
      Thread.Sleep(1000);

    }
  }
}

