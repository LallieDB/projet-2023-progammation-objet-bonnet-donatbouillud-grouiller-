
public class Plateau
{
  //lettre pour l'affichage des joueurs
  //majuscule si ils tiennent le ballon
  private static readonly string ICONE_COUREUR_SANS_BALLON = "c";
  private static readonly string ICONE_COUREUR_AVEC_BALLON = "C";
  private static readonly string ICONE_BRUTE_SANS_BALLON = "b";
  private static readonly string ICONE_BRUTE_AVEC_BALLON = "B";
  private static readonly string ICONE_DEFENSEUR_SANS_BALLON = "d";
  private static readonly string ICONE_DEFENSEUR_AVEC_BALLON = "D";
  private static readonly string ICONE_GARDIEN_SANS_BALLON = "g";
  private static readonly string ICONE_GARDIEN_AVEC_BALLON = "G";
  public List<Joueur> Joueurs { get; private set; }

  //Ballon
  private static readonly string ICONE_BALLON = "*";
  public Ballon BallonPlateau { get; protected set; }

  public List<bool> VientDeMourir { get; protected set; }
  public string[,] PlateauJeu { get; private set; } //Tableau qui sert à l'affichage
  public string[,] PlateauVide { get; private set; } //Tableau qui sert pour l'initialisation à chaque tour
  public int Ligne { get; private set; } // Nb de ligne du plateau
  public int Colonne { get; private set; } //Nb de colonne du plateau
  public int[,] PositionJoueur { get; private set; } //Tableau qui sert pour l'affichage en couleur des émojis pour les joueurs et de leurs couleurs de fond
                                                     //Si la case du tableau est nulle, il n'y a pas de joueur.
                                                     //Si le joueur est en bonne santé, la valeur est 3, en moyenne santé 2 et en santé critique 1. Sert pour la couleur du fond.
                                                     //Les joueurs morts compte pour 0. Les cases vides n'apparaissent pas et compte pour 0
  public Inventaire InventaireE1 { get; set; }
  public Inventaire InventaireE2 { get; set; }
  public int NbToursTotal { get; set; }
  public int NbTours { get; set; }
  public Plateau(Ballon ballonPlateau, int ligne, int colonne)
  {
    InventaireE1 = new Inventaire(1, 1);
    InventaireE2 = new Inventaire(1, 1);
    BallonPlateau = ballonPlateau;
    Ligne = ligne;
    Colonne = colonne;
    Joueurs = new List<Joueur>();
    VientDeMourir = new List<bool>();
    for (int i = 0; i < 8; i++)
    {
      VientDeMourir.Add(false);
    }
    PlateauVide = new string[ligne, colonne];
    PlateauVide = InitialiserTableauVide(PlateauVide); //Initialisation du plateau de jeu vide
    this.PlateauJeu = new string[ligne, colonne];
    PositionJoueur = new int[ligne, colonne];
    PlateauJeu = InitialiserTableauVide(PlateauJeu);
    NbTours = 0;
    NbToursTotal = 0;//en attendant, sera changer avec la simulation
  }
  public string[,] InitialiserTableauVide(string[,] plateau)
  {
    //ligne doit être un multiple de 3, colonne doit être un nb impair
    //divisionTier permet de séparer le plateau en 3 partie égale, 
    //cela permettra une répartition plus homogène pour les cages de but
    List<int> divisionTier = new List<int>();
    for (int k = 0; k < plateau.GetLength(0) / 3; k++)
      divisionTier.Add(plateau.GetLength(0) / 3 + k);

    //ligne 0 et dernière
    for (int j = 0; j < plateau.GetLength(1); j++)
    {
      if (j % 2 == 0) //j pair
      {
        plateau[0, j] = " ";
        plateau[plateau.GetLength(0) - 1, j] = " ";
      }
      else
      {
        plateau[0, j] = "-";
        plateau[plateau.GetLength(0) - 1, j] = "-";
      }
    }

    //ligne 1 à avant dernière
    for (int i = 1; i < plateau.GetLength(0) - 1; i++)
    {
      for (int j = 0; j < plateau.GetLength(1); j++)
      {
        if (((j == 1 || j == plateau.GetLength(1) - 2) && divisionTier.Contains(i) == false) || (j == plateau.GetLength(1) / 2))
          plateau[i, j] = ":";
        else
          plateau[i, j] = " ";
      }
    }
    return plateau; // renommer
  }

  public void AjouterJoueur(Joueur joueur)
  {
    Joueurs.Add(joueur);
  }
  public void RetirerJoueur(Joueur joueur)
  {
    Joueurs.Remove(joueur);
  }

  public void NettoyagePlateauJeu()
  {
    //Nettoyage du plateau
    // on nettoie à chaque tour le terrain, on placera ensuite les joueurs et le ballon avec leur nouvelle position
    for (int i = 0; i < PlateauJeu.GetLength(0); i++)
    {
      for (int j = 0; j < PlateauJeu.GetLength(1); j++)
      {
        if (PlateauJeu[i, j] != " ")
          PlateauJeu[i, j] = PlateauVide[i, j];
        PositionJoueur[i, j] = -1; //On nettoie également le tableau des joueurs en remettant tout à -1.
      }
    }
    //on nettoie aussi la liste des morts du tours
    for (int i = 0; i < 8; i++)
    {
      Joueurs[i].MortDansLeTour = false;
    }
  }

  public string MortDurantLeTour()
  {
    string annonceMort = "";
    //mise à jour de VientDeMourir
    for (int i = 0; i < VientDeMourir.Count(); i++)
    {
      VientDeMourir[i] = Joueurs[i].MortDansLeTour;
    }

    //texte final pour tous les joueurs qui viennent de mourir durant le tour
    for (int i = 0; i < VientDeMourir.Count(); i++)
    {
      if (VientDeMourir[i] == true)
      {
        // Le joueur perd le contrôle de la balle si il meurt
        // TexteJoueur += "  _______\n";
        // TexteJoueur += " |       |\n";
        // TexteJoueur += "| []  []  |\n";
        // TexteJoueur += " |       |\n";
        // TexteJoueur += "  |_|_|_|\n";
        if (Joueurs[i].EquipeTroll == false)
          annonceMort += $"Oh non! {Joueurs[i].Prenom} est mort au combat :'( \n";
        else
          annonceMort += $"Ah! Ah! {Joueurs[i].Prenom}, ce maudit troll est mort. Bien fait pour lui ! o(^v^)o \n";
      }
    }
    return annonceMort;
  }


  private void AffichageJoueurBallon()
  {
    for (int i = 0; i < Joueurs.Count(); i++)
    { //on affiche ensuite la position actuel des joueurs, et on note leur numero
      if (Joueurs[i].Mort == false)
      {
        switch (Joueurs[i].ClasseJoueur())
        {
          case "coureur":
            if (BallonPlateau.Ligne == Joueurs[i].Ligne && BallonPlateau.Colonne == Joueurs[i].Colonne)
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_COUREUR_AVEC_BALLON;
            else
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_COUREUR_SANS_BALLON;
            break;
          case "defenseur":
            if (BallonPlateau.Ligne == Joueurs[i].Ligne && BallonPlateau.Colonne == Joueurs[i].Colonne)
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_DEFENSEUR_AVEC_BALLON;
            else
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_DEFENSEUR_SANS_BALLON;
            break;
          case "brute":
            if (BallonPlateau.Ligne == Joueurs[i].Ligne && BallonPlateau.Colonne == Joueurs[i].Colonne)
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_BRUTE_AVEC_BALLON;
            else
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_BRUTE_SANS_BALLON;
            break;
          case "gardien":
            if (BallonPlateau.Ligne == Joueurs[i].Ligne && BallonPlateau.Colonne == Joueurs[i].Colonne)
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_GARDIEN_AVEC_BALLON;
            else
              PlateauJeu[Joueurs[i].Ligne, Joueurs[i].Colonne] = ICONE_GARDIEN_SANS_BALLON;
            break;
        }
        PositionJoueur[Joueurs[i].Ligne, Joueurs[i].Colonne] = i + 1; //On ajoute le numéro du joueur (de la liste Joueurs) pour les colorer plus tard en fonction de leur équipe 
      }
    }
    // on place le ballon si celui-ci n'est pas encore placé
    if (PlateauJeu[BallonPlateau.Ligne, BallonPlateau.Colonne] != $"{ICONE_COUREUR_AVEC_BALLON}" && PlateauJeu[BallonPlateau.Ligne, BallonPlateau.Colonne] != $"{ICONE_DEFENSEUR_AVEC_BALLON}" && PlateauJeu[BallonPlateau.Ligne, BallonPlateau.Colonne] != $"{ICONE_BRUTE_AVEC_BALLON}" && PlateauJeu[BallonPlateau.Ligne, BallonPlateau.Colonne] != $"{ICONE_GARDIEN_AVEC_BALLON}")
      PlateauJeu[BallonPlateau.Ligne, BallonPlateau.Colonne] = $"{ICONE_BALLON}";
  }
  private void ColorationDesCases(int ligne, int colonne)
  { //NE SERT PLUS, AFFICHAGE PEU PRATIQUE 
    // permet de mettre des couleurs au position des joueurs
    Console.ResetColor();
    //le joueur est mort, la case sera grise
    if (PositionJoueur[ligne, colonne] == -1)
    {
      Console.BackgroundColor = ConsoleColor.White;
      Console.Write(PlateauJeu[ligne, colonne]);
      Console.BackgroundColor = ConsoleColor.Black;

    }
    else if (PositionJoueur[ligne, colonne] > 0)
    {
      //couleur des joueurs vivants
      switch (PositionJoueur[ligne, colonne])
      { //On choisit le fond en fonction de l'état du joueur
        case 1: //zone critique, plus beaucoup de PV
          Console.BackgroundColor = ConsoleColor.Red;
          Console.Write(PlateauJeu[ligne, colonne]);
          break;

        case 2: // PV moyen
          Console.BackgroundColor = ConsoleColor.DarkYellow;
          Console.Write(PlateauJeu[ligne, colonne]);
          break;

        case 3: // Grand nb de PV restant
          Console.BackgroundColor = ConsoleColor.Green;
          Console.Write(PlateauJeu[ligne, colonne]);
          break;
      }
      Console.BackgroundColor = ConsoleColor.Black;

    }
    else if (PositionJoueur[ligne, colonne] == 0)
    { // si on ne veut pas mettre de couleur
      Console.Write(PlateauJeu[ligne, colonne]);
    }
    Console.ResetColor();
  }
  private void ColorationEquipe(int ligne, int colonne)
  {
    //coloration par équipe
    // if (PositionJoueur[ligne, colonne] != -1)
    //on colore seulement les joueurs encore en vie
    {
      if (PositionJoueur[ligne, colonne] > 0 && PositionJoueur[ligne, colonne] < 5) //équipe 1
      {
        if (BallonPlateau.Ligne == ligne && BallonPlateau.Colonne == colonne)
          Console.BackgroundColor = ConsoleColor.White;

        else
          Console.BackgroundColor = ConsoleColor.Yellow;
      }
      else if (PositionJoueur[ligne, colonne] >= 5)
      {
        if (BallonPlateau.Ligne == ligne && BallonPlateau.Colonne == colonne)
          Console.BackgroundColor = ConsoleColor.Blue;
        else
          Console.BackgroundColor = ConsoleColor.Magenta;
      }
    }
    Console.Write(PlateauJeu[ligne, colonne]);
    Console.BackgroundColor = ConsoleColor.Black;
  }
  private void CouleurPVLegende(int numeroJoueur)
  {
    //Affichage des PV des joueurs de façon en coloré en fonction de leur état
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(": ");
    switch (Joueurs[numeroJoueur].EtatDuJoueur)
    {
      case -1: //mort=gris
        Console.ForegroundColor = ConsoleColor.DarkGray;
        break;
      case 1://critique=rouge
        Console.ForegroundColor = ConsoleColor.Red;
        break;
      case 2: //moyen=jaune
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        break;
      case 3: //correct=vert
        Console.ForegroundColor = ConsoleColor.Green;
        break;
    }
    Console.Write($"{Joueurs[numeroJoueur].PointDeVie}/{Joueurs[numeroJoueur].PointdeVieInitial}");
    Console.ForegroundColor = ConsoleColor.White;//on remet en blanc pour la suite du texte
  }

  private void AffichageLegende(int numeroEquipe, int numeroJoueur)
  {
    Console.Write($"     | J{numeroJoueur - 4 * (numeroEquipe - 1) + 1} : {Joueurs[numeroJoueur].ClasseJoueur()}");
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.Write($" {Joueurs[numeroJoueur].Prenom} ");
    Console.ForegroundColor = ConsoleColor.White;
    CouleurPVLegende(numeroJoueur);
  }

  private void AffichageInformation(int i)
  {
    //à chaque tour, on donne au joueur plusieurs informations, elles apparaissent à droite du terrain
    //On peut voir le nom des joueurs de chaque équipe ainsi que leur nb de PV restant
    // Le joueur peut aussi connaitre les objets restants dans son inventaire
    switch (i)
    {
      case 0:
        Console.WriteLine("     | --------------");
        break;
      case 2:
        Console.WriteLine($"     | Equipe n°1 | Nb de Baie = {InventaireE1.NbBaie} | Nb de Baie Magique = {InventaireE1.NbBaieMagique}");
        break;
      case 3:
        AffichageLegende(1, 0);
        AffichageLegende(1, 1);
        Console.Write("\n");
        break;

      case 4:
        AffichageLegende(1, 2);
        AffichageLegende(1, 3);
        Console.Write("\n");
        break;

      case 6:
        Console.WriteLine($"     | Equipe n°2 | Nb de Baie = {InventaireE2.NbBaie} | Nb de Baie Magique = {InventaireE2.NbBaieMagique}");
        break;
      case 7:
        AffichageLegende(2, 4);
        AffichageLegende(2, 5);
        Console.Write("\n");
        break;
      case 8:
        AffichageLegende(2, 6);
        AffichageLegende(2, 7);
        Console.Write("\n");
        break;
      default:
        Console.WriteLine("     |");
        break;
    }
  }

  public override string ToString()
  {
    //On initialise d'abord le plateauvide en prenant en compte la position actuelle des joueurs et du ballon
    AffichageJoueurBallon();

    //On affiche ensuite le terrain en entier à l'aide du PlateauVide
    //bordure du haut
    string terrain = " -";
    for (int n = 0; n < PlateauJeu.GetLength(1); n++)
      terrain += "—";
    terrain += $"-      | INFORMATIONS | TOURS : {NbTours + 1}/{NbToursTotal}\n";

    //premier tier 
    Console.Write(terrain);
    terrain = "";
    for (int i = 0; i < PlateauJeu.GetLength(0) / 3; i++)
    {
      Console.Write(" |");
      for (int j = 0; j < PlateauJeu.GetLength(1); j++)
      {
        ColorationEquipe(i, j);
      }
      Console.Write("| ");
      AffichageInformation(i);
    }

    //deuxième tier avec but
    for (int i = PlateauJeu.GetLength(0) / 3; i < 2 * PlateauJeu.GetLength(0) / 3; i++)
    {
      Console.Write("‡‡");
      for (int j = 0; j < PlateauJeu.GetLength(1); j++)
      {
        ColorationEquipe(i, j);

      }
      Console.Write("‡‡");
      AffichageInformation(i);
    }
    //dernier tier, comme le premier
    for (int i = 2 * PlateauJeu.GetLength(0) / 3; i < PlateauJeu.GetLength(0); i++)
    {
      Console.Write(" |");
      for (int j = 0; j < PlateauJeu.GetLength(1); j++)
      {
        ColorationEquipe(i, j);
      }
      Console.Write("| ");
      AffichageInformation(i);
    }
    //bordure du bas = bordure du haut
    terrain += " -";
    for (int n = 0; n < PlateauJeu.GetLength(1); n++)
      terrain += "—";
    terrain += "-      |\n";
    return terrain;
  }
}


//exemple
//   |0:1:2:3:4:5:6:7:8:9:0:1|
//   -———————————————————————-
//   | - - - - - - - - - - - |
//   | :         :         : |
//   | :         :         : |
//  ‡‡           :           ‡‡
//  ‡‡           :           ‡‡
//  ‡‡           :           ‡‡
//   | :         :         : |
//   | :         :         : | 
//   | - - - - - - - - - - - |
//   -———————————————————————-

