
public class Gardien : Joueur
{
  private static readonly int RECONNAITRE_ALLIE = 85;

  public int Niveau { get; set; }

  // --------------------------- CONSTRUCTEUR ---------------------------------------------
  public Gardien(string prenom, int ligne, int colonne, int pointdevie, int niveau, bool EquipeGauche)
  : base(prenom, ligne, colonne, pointdevie, EquipeGauche)
  {
    Niveau = niveau; // en fonction du niveau du gardien, celui est plus ou moins efficace pour intercepter le ballon

    // NIVEAU 1 : le gardien a un déplacement aléatoire dans sa zone. 
    // Si le ballon est à la même position que lui, il a 50% de réussir à l'attraper

    // NIVEAU 2 : le gardien a un déplacement aléatoire dans sa zone, il n'est jamais statique
    // Si le ballon est à la même position que lui, il a 65% de réussir à l'attraper

    // NIVEAU 3 : le gardien suit le ballon, se déplace en fonction de sa position
    // Si le ballon est à la même position que lui, il a 85% de réussir à l'attraper
  }

  // --------------------------- DEPLACEMENT ---------------------------------------------
  private void Descendre(Plateau plateau)
  {
    if (Ligne != 2 * plateau.PlateauJeu.GetLength(0) / 3 - 1)
    {
      Ligne += 1;
    }
    //si niv 1, le gardien reste statique
    //en revanche si niv 2, le gardien ne s'arrete pas et change de direction
    else
    {
      if (Niveau == 2)
        Monter(plateau);
    }
  }

  private void Monter(Plateau plateau)
  {
    if (Ligne != plateau.PlateauJeu.GetLength(0) / 3)
    {
      Ligne -= 1;
    }
    //si niv 1, le gardien reste statique
    //en revanche si niv 2, le gardien ne s'arrete pas et change de direction
    else
    {
      if (Niveau == 2)
        Descendre(plateau);
    }
  }

  public override void Deplacer(Plateau plateau, Ballon ballon)
  {
    if (ballon.Colonne == Colonne && ballon.Ligne >= plateau.PlateauJeu.GetLength(0) / 3 && ballon.Ligne <= plateau.PlateauJeu.GetLength(0) * 2 / 3 - 1 && Balle == false)
    { //Si le ballon est dans les buts et que le gardien ne la possède pas, il essaie de l'empêcher de marquer
      int distance = Math.Abs(Ligne - ballon.Ligne);
      if (distance <= 1)
      { //Si le gardien est à proximité du ballon, il va sur la ces du ballon et essaie de l'attraper
        Ligne = ballon.Ligne;
        Colonne = ballon.Colonne;
        bool reussite = AttraperBallon(); //On lance un dé pour savoir si le gardien réussit à empêcher le but
        if (reussite == true)
        {
          TexteJoueur += $"OUI! {Prenom} a rattraper la balle avant qu'il ne soit trop tard.\n";
          Balle = true;
          ballon.BallonStatique();
        }
        else
          TexteJoueur += $"NON! {Prenom} n'a pas réussit à rattraper la balle avant qu'il ne soit trop tard.\n";
      }
    }
    else if (Balle == true)
    { //Si le gardien a la balle, il fait la passe aux joueurs le plus proche. 
      //Néanmoins le gardien ne connait pas très bien ses coéquipiers et a une probabilité non nulle 
      //de faire la passe à un adversaire
      FairePasse(plateau, ballon);
    }
    else
    {
      //Le gardien se déplace normalement
      //niv débutant : le gardien se déplace (haut ou bas) de manière aléatoire dans sa zone 
      //(ne prend pas en compte la position du ballon)
      if (Niveau != 3)
      {
        int deplacement = rnd.Next(0, 2);
        switch (deplacement)
        {
          case 0:
            Monter(plateau);
            break;

          case 1:
            Descendre(plateau);
            break;
        }
      }
      else // le gardien est niv 3, il suit la le mvt de la balle
      {
        if (plateau.BallonPlateau.Ligne < plateau.PlateauJeu.GetLength(0) / 3)
          Ligne = plateau.PlateauJeu.GetLength(0) / 3;
        else if (plateau.BallonPlateau.Ligne > 2 * plateau.PlateauJeu.GetLength(0) / 3)
          Ligne = 2 * plateau.PlateauJeu.GetLength(0) / 3 - 1;
        else
          Ligne = plateau.BallonPlateau.Ligne;
      }
    }
  }

  // ---------------------------- LANCER LE BALLON ---------------------------------------------

  public void FairePasse(Plateau plateau, Ballon ballon)
  {
    bool joueurAdverseVivant = false;
    bool joueurAllieVivant = false;
    int distanceAllieMin = plateau.PlateauJeu.GetLength(0) + plateau.PlateauJeu.GetLength(1);
    int distanceAdversaireMin = plateau.PlateauJeu.GetLength(0) + plateau.PlateauJeu.GetLength(1);
    int distanceAllie;
    int numeroJoueurListe = 0;
    int numeroAllie = 0;
    int numeroAdversaire = 0;
    foreach (Joueur joueur in plateau.Joueurs)
    { //On regarde le joueur adverse et le joueur allié le plus proche
      if (joueur.Mort == false && joueur.Numero != Numero && joueur.EquipeTroll == EquipeTroll)
      {
        joueurAllieVivant = true;
        distanceAllie = Math.Abs(joueur.Colonne - Colonne) + Math.Abs(joueur.Ligne - Ligne);
        if (distanceAllie < distanceAllieMin)
        {
          distanceAllieMin = distanceAllie;
          numeroAllie = numeroJoueurListe;
        }
        else
        {//Fin du jeu aux prochain tour
        }
      }
      numeroJoueurListe++;
    }
    if (joueurAdverseVivant == true && joueurAllieVivant == true)
    {
      bool fairepasseallie = ReussirLancerDe(RECONNAITRE_ALLIE);
      if (fairepasseallie == false)
      { //Le gardien fait la passe à l'allié le plus proche
        Balle = false;
        ballon.ChangerCoordonneesBallon(plateau.Joueurs[numeroAdversaire].Ligne, plateau.Joueurs[numeroAdversaire].Colonne);
        plateau.Joueurs[numeroAdversaire].Balle = true;
        TexteJoueur += $"{Prenom} donne le ballon a {plateau.Joueurs[numeroAdversaire].Prenom}.Il ne connait vraiment pas son équipe...\n";
      }
      else
      {
        Balle = false;
        ballon.ChangerCoordonneesBallon(plateau.Joueurs[numeroAllie].Ligne, plateau.Joueurs[numeroAllie].Colonne);
        plateau.Joueurs[numeroAllie].Balle = true;
        TexteJoueur += $"{Prenom} donne le ballon a {plateau.Joueurs[numeroAllie].Prenom}.\n";
      }
    }
    else
    {//Normalement fin du jeu, mais peut être à remplir différement
    }
  }


  // ---------------------------- RATTRAPER LE BALLON ---------------------------------------------
  public bool AttraperBallon()
  {
    //cette fonction n'est à utiliser que si le ballon et le gardien on la même position
    //on part du principe que cela a été vérifier avant
    //en fonction du niv du gardien, celui-ci à plus ou moins de chance de réussir
    bool verifier = false;
    int tirage = rnd.Next(1, 101);
    if ((Niveau == 1 && tirage <= 50) || (Niveau == 2 && tirage <= 65) || (Niveau == 3 && tirage <= 85))
      verifier = true;
    return verifier;
  }
  // --------------------------- DESCRIPTION RELATIVE A UN GARDIEN ---------------------------------------------
  public override string ClasseJoueur()
  {
    return "gardien";
  }
  public override string ToString()
  {
    return "Le gardien est :" + base.ToString();
  }
}

