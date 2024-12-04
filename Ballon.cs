// Les valeurs sous-jacentes implicites sont des entiers entre 0 et 3
public enum Orientation { Haut, Bas, Droite, Gauche, HautDroite, BasDroite, BasGauche, HautGauche }

public class Ballon
{
    public int Ligne { get; private set; }
    public int Colonne { get; private set; }
    public List<int> Score { get; private set; } // La 1ere valeur correspond au score des trolls et la 2eme des humains
    public bool Impulsion { get; set; } // Se qui donne le 1er mouvement au ballon
    public bool? VersLaDroite { get; private set; } //Si vrai va vers la droite, si faux vers la gauche
    public bool? VersLeHaut { get; private set; } //Si vrai va vers le haut, sinon vers le bas
    public bool? DiagonalePositive { get; private set; } //Si vrai va vers le haut à droite, sinon vers le bas à gauche
    public bool? DiagonaleNegative { get; private set; } //Si vrai va vers le haut à gauche, sinon vers le bas à droite
    public Orientation Direction { get; private set; } // C'est l'orientation dans le plan qu'on défini par une énumération. 

    // --------------------------- CONSTRUCTEUR ---------------------------------------------
    public Ballon(int ligne, int colonne)
    {
        Ligne = ligne;
        Colonne = colonne;
        Score = new List<int> { 0, 0 };
        Impulsion = false;
    }

    // --------------------------- DEPLACEMENT ---------------------------------------------
    public void Roule(Plateau plateau)
    { //Fonction qui déplace en roulant le ballon selon sa direction. Si le ballon n'a pas d'impulsion, il ne se déplace pas
        if (Impulsion == true)
        { //Si le ballon a une impulsion, il roule dans où on lui "indique". Sinon il reste statique
            if (VersLaDroite == true)
            {
                Direction = Orientation.Droite;
                ModifierPosition(Direction);
            }
            else if (VersLaDroite == false)
            {
                Direction = Orientation.Gauche;
                ModifierPosition(Direction);
            }
            if (VersLeHaut == true)
            {
                Direction = Orientation.Haut;
                ModifierPosition(Direction);
            }
            else if (VersLeHaut == false)
            {
                Direction = Orientation.Bas;
                ModifierPosition(Direction);
            }
            if (DiagonalePositive == true)
            {
                Direction = Orientation.HautDroite;
                ModifierPosition(Direction);
            }
            else if (DiagonalePositive == false)
            {
                Direction = Orientation.BasGauche;
                ModifierPosition(Direction);
            }
            if (DiagonaleNegative == true)
            {
                Direction = Orientation.HautGauche;
                ModifierPosition(Direction);
            }
            else if (DiagonaleNegative == false)
            {
                Direction = Orientation.BasDroite;
                ModifierPosition(Direction);
            }
            DetecterHorsZoneEtReplacerBallon(plateau);
        }
    }
    public void DeplacerBallon(Plateau plateau)

    { //Fonction qui déplace le ballon aux coordonnées choisit
        bool avoirBalle = AuMoinsUnJoueurAvecLaBalle(plateau);
        if (avoirBalle == false)
        {
            Roule(plateau);
        }
        else
        {
            Impulsion = false;
        }
    }

    public bool AuMoinsUnJoueurAvecLaBalle(Plateau plateau)
    {
        bool verifier = false;
        int k = 0;
        while (k < plateau.Joueurs.Count)
        {
            if (plateau.Joueurs[k].Balle == true)
            { // On parcrous l'ensemble des joueurs, dès qu'au moins 1 à la balle on retourne vrai. 
                Ligne = plateau.Joueurs[k].Ligne;
                Colonne = plateau.Joueurs[k].Colonne;
                verifier = true;
            }
            k++;
        }
        return verifier;
    }
    public void ModifierPosition(Orientation direction)
    {
        switch (direction)
        { // On se deplace dans toutes les directions du plan avec les points cardinaux et leurs diagonales.
            case Orientation.Haut:
                Ligne--;
                VersLeHaut = true;
                break;
            case Orientation.Bas:
                Ligne++;
                VersLeHaut = false;
                break;
            case Orientation.Droite:
                Colonne++;
                VersLaDroite = true;
                break;
            case Orientation.Gauche:
                Colonne--;
                VersLaDroite = false;
                break;
            case Orientation.HautDroite:
                Ligne--;
                Colonne++;
                DiagonalePositive = true;
                break;
            case Orientation.BasDroite:
                Colonne++;
                Ligne++;
                DiagonaleNegative = false;
                break;
            case Orientation.BasGauche:
                Colonne--;
                Ligne++;
                DiagonalePositive = true;
                break;
            case Orientation.HautGauche:
                Ligne--;
                Colonne--;
                DiagonaleNegative = true;
                break;
        }
    }

    public void ChangerCoordonneesBallon(int ligne, int colonne)
    { // On peut choisir l'emplacement précis du ballon
        Ligne = ligne;
        Colonne = colonne;
    }
    public void BallonStatique()
    { // Fonction qui reinitialise les paramètres de déplacement du ballon
      //Cette fonction sert dans le cas où le ballon ne bouge pas ou dans le cas où il est contrôlé par un joueur
        Impulsion = false;
        VersLaDroite = null;
        VersLeHaut = null;
        DiagonaleNegative = null;
        DiagonalePositive = null;
    }

    // --------------------------- SORTIE DU BALLON ---------------------------------------------
    public void RemiseAZero(Plateau plateau, int avantage)
    {
        //avantage permet de déterminer dans le calcul la position du ballon au niveau de la colonne
        //pour gagner en optimisation, on écrira 0 si il n'y a pas d'avantage
        // -1 si c'est l'équipe 1 qui a l'avantage
        // 1 si c'est l'équipe 2 qui a l'avantage
        BallonStatique(); //Le ballon redevient statique
        Ligne = (plateau.Ligne / 2);
        Colonne = plateau.Colonne / 2 + avantage * (plateau.Ligne / 3) - avantage;
        plateau.Joueurs[0].Ligne = plateau.Ligne / 2;
        plateau.Joueurs[0].Colonne = plateau.Colonne / 2 - plateau.Ligne / 3;
        plateau.Joueurs[1].Ligne = plateau.Ligne / 3 - 1;
        plateau.Joueurs[1].Colonne = plateau.Colonne / 2 - 2 * plateau.Ligne / 3;
        plateau.Joueurs[2].Ligne = 2 * plateau.Ligne / 3;
        plateau.Joueurs[2].Colonne = plateau.Colonne / 2 - 2 * plateau.Ligne / 3;
        plateau.Joueurs[3].Ligne = plateau.Ligne / 2;
        plateau.Joueurs[3].Colonne = 1;

        plateau.Joueurs[4].Ligne = plateau.Ligne / 2;
        plateau.Joueurs[4].Colonne = plateau.Colonne / 2 + plateau.Ligne / 3;
        plateau.Joueurs[5].Ligne = plateau.Ligne / 3 - 1;
        plateau.Joueurs[5].Colonne = plateau.Colonne / 2 + 2 * plateau.Ligne / 3;
        plateau.Joueurs[6].Ligne = 2 * plateau.Ligne / 3;
        plateau.Joueurs[6].Colonne = plateau.Colonne / 2 + 2 * plateau.Ligne / 3;
        plateau.Joueurs[7].Ligne = plateau.Ligne / 2;
        plateau.Joueurs[7].Colonne = plateau.Colonne - 1;

    }

    public void DetecterHorsZoneEtReplacerBallon(Plateau plateau)
    {
        if (MarquerUnBut(plateau) == true || HorsZoneBallon(plateau) == true)
        { // On verifie si on a marqué un but ou qu'on est hors zone. Dans l'un de ces 2 cas, on positionne la balle au centre. 
            RemiseAZero(plateau, 0);
        }
    }
    public bool HorsZoneBallon(Plateau plateau)
    {
        if ((Ligne == 0 || Ligne == plateau.Ligne - 1) || (Colonne == 0 || Colonne == plateau.Colonne - 1) || ((Ligne < plateau.Ligne / 3 || Ligne >= 2 * plateau.Ligne / 3) && (Colonne == 1 || Colonne == plateau.Colonne - 2)))
        {   //Si le ballon se situe sur les bords, retourne true, c'est à dire qu'il est hors zone. 
            return true;
        }
        else
            return false;
    }

    // --------------------------- BUT ET SCORE ---------------------------------------------
    public bool MarquerUnBut(Plateau plateau)
    {
        bool but = false;

        List<int> divisionTier = new List<int>();
        for (int k = 0; k < plateau.Ligne / 3; k++)
            divisionTier.Add(plateau.Ligne / 3 + k);

        if (Colonne == 0 && divisionTier.Contains(Ligne))
        {//vérifie si le ballon est dans les cases du gardien de but de gauche (1)
            ChangerScore(1);
            but = true;
        }
        else if (Colonne == plateau.Colonne - 1 && divisionTier.Contains(Ligne))
        {//vérifie si le ballon est dans les cases du gardien de but de droite (2)
            ChangerScore(2);
            but = true;
        }
        return but;
    }
    private void ChangerScore(int numeroEquipe)
    {
        switch (numeroEquipe)
        {
            case 1: // si le ballon est dans le but de l'équipe 1, alors le point est pour l'équipe adverse (2)
                Score[1] += 1;
                break;
            case 2: // si le ballon est dans le but de l'équipe 2, alors le point est pour l'équipe adverse (1)
                Score[0] += 1;
                break;
        }
    }
}