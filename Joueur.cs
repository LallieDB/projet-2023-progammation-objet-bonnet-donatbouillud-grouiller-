
public abstract class Joueur
{
    static protected readonly int FORCEDEFRAPPE = 3; //Nombre de point de vie que fait perdre un coureur en tappant quelqu'un
    static protected readonly int PROBA_DE_LANCER_LA_BALLE = 60;
    static protected readonly int PROBA_DE_RECUPERER_LA_BALLE = 70;
    static protected readonly int PROBA_DE_SE_DEPLACER = 90;

    static protected readonly int PROBA_DE_FRAPPER_BRUTE = 90;
    static protected readonly int PROBA_DE_FRAPPER_NORMALE = 60;
    static private readonly int PROBA_RECEPTION_BALLON_COUREUR = 80;
    static private readonly int PROBA_RECEPTION_BALLON_DEFENSEUR = 90;
    static private readonly int PROBA_RECEPTION_BALLON_BRUTE = 40;
    static private readonly int PROBA_RECEPTION_BALLON_GARDIEN = 70;
    static private int numeroJoueur = 0;
    public int Numero { get; } //Permet d'avoir un identifiant unique pour chaque joueur

    public string TexteJoueur { get; protected set; } //Sert pour afficher les commentaires en bas du terrain à chaque tour de simulation
    static protected Random rnd = new Random(); //Variable aléatoire qui sert pour les lancer de dés
    public string Prenom { get; }
    public int Ligne { get; set; }
    public int Colonne { get; set; }
    public int PointDeVie { get; set; } //Nombre de points de vie du joueur
    public int PointdeVieInitial { get; } //Nombre de points de vie initial du joueur. Ne change jamais.
    public bool Balle { get; set; } // Booléen qui est égal à true quand le joueur à la balle
    public bool Mort { get; private set; } //Booléen qui devient égal à true à la mort du joueur
    public bool MortDansLeTour { get; set; }
    public bool EquipeTroll { get; set; }
    public int EtatDuJoueur { get; private set; } //Entier qui retourne l'état du joueur. Si le joueur est en bonne santé, il est égal à 3.
                                                  //Si le joueur est en moyenne santé, l'entier est égal à 2. Si la santé du joueur est critique, elle est égale à 1.

    // ------------------------------ CONSTRUCTEUR --------------------
    public Joueur(string prenom, int ligne, int colonne, int pointdevie, bool EquipeTroll)
    {
        Prenom = prenom;
        Ligne = ligne;
        Colonne = colonne;
        PointDeVie = pointdevie;
        PointdeVieInitial = pointdevie;
        if (pointdevie < 1) //Si jamais le joueur n'a pas de point de vie, on l'initialise à 10 pdv
        {
            pointdevie = 10;
        }
        Mort = false; //Le joueur n'est pas mort au début
        MortDansLeTour = false;
        Balle = false; //Le joueur n'a pas la balle au début
        this.EquipeTroll = EquipeTroll;
        EtatDuJoueur = 3; //Le joueur commence avec toute sa vie, il est donc en bonne santé
        Numero = ++numeroJoueur;

        TexteJoueur = ""; //On a aucun commentaire à faire au début
    }

    //------------------------- CLASSES ABSTRAITES -------------------------------------
    public abstract void Deplacer(Plateau plateau, Ballon ballon);

    public abstract string ClasseJoueur(); //Retourne de quelle classe est le joueur considéré
    public virtual void LancerBallon(Plateau plateau, Ballon ballon)
    { //Fonction qui décrit les différents comportement des joueurs quand il lance le ballon
      //Par défaut, les joueurs lancent le ballon vers les buts
        LancementDuBallonVersBut(plateau, ballon);
    }
    // --------------------------- TESTS EFFECTUES SUR LA JOUEUR ---------------------
    public bool ReussirLancerDe(int proba)
    { //Fonction qui retourne true si le lancer de dé est réissit, false sinon
      //Un lancer de dé est considéré comme réussit si le résultat sur un dé 100 est inférieur ou égal 
      //à la proba passé en paramètre
        int reussite = rnd.Next(0, 101);
        if (reussite > proba)
            return false;
        else
            return true;
    }
    public bool HorsTerrain(Plateau plateau, int ligne, int colonne)
    {
        //Fonction utile pour la classe Joueur
        //Fonction qui permet de savoir si la personne a la ligne et à la colonne choisie est dans le terrain
        //Fonction a utilisé comme test avant de déplacer le joueur
        //Elle retourne false si le joueur sera hors terrain aux coordonnées choisies et true sinon
        if (ligne == 0 || ligne >= plateau.PlateauJeu.GetLength(0) - 1 || colonne == 0 || colonne >= plateau.PlateauJeu.GetLength(1) - 1)
        {   //Si le joueur se situe sur les bords, retourne false
            return false;
        }


        for (int i = plateau.PlateauVide.GetLength(0) / 3; i < 2 * plateau.PlateauVide.GetLength(0) / 3; i++)
        {
            if ((colonne == 1 && Ligne == i) || (colonne == plateau.PlateauVide.GetLength(1) - 2 && Ligne == i))
            {//vérifie si le joueur est dans les cases du gardien de but de gauche ou de droite
                return false;
            }
        }
        return true;
    }

    // --------------------- SANTE ET POINT DE VIE DU JOUEUR -----------------------
    public void SanteJoueur()
    {
        if (PointDeVie <= PointdeVieInitial && PointDeVie > PointdeVieInitial / 2)
        { // Si le joueur a un nombre de point de vie supérieur à la moitié de ses pv, on considère qu'il va bien. 
            EtatDuJoueur = 3; //Si le joueur est en bonne santé, son état est égal à 3.
        }
        else if (PointDeVie <= PointdeVieInitial / 2 && PointDeVie > PointdeVieInitial / 4)
            EtatDuJoueur = 2; //Si le joueur est en moyenne bonne santé, son état est égal à 2.
        else if (PointDeVie > PointdeVieInitial)
            TexteJoueur += $"Erreur, {Prenom} ne peut pas avoir plus de pdv que sa santé max";
        else
            EtatDuJoueur = 1; //Si le joueur est en mauvaise santé, son état est égal à 1.
    }
    public void PerdrePV(int nbPdv)
    {
        //Fonction qui enlève aux joueurs le nombre de points de vie donné en entrée
        if (PointDeVie - nbPdv > 0)
            PointDeVie -= nbPdv;
        else
            PointDeVie = 0;
        SanteJoueur(); //mise à jour de l'état du joueur
        if (PointDeVie == 0)
        {
            Mort = true;
            MortDansLeTour = true;
            EtatDuJoueur = -1;
            Balle = false;
        }
    }
    public void Revivre()
    { //En cas d'utilisation de baie magique ou de soin, le joueur n'est plus considéré mort
        Mort = false;
        EtatDuJoueur = 3;
    }

    // --------------------     DEPLACEMENT POSSIBLE DU JOUEUR ----------------------------

    public void Deplacement(Plateau plateau, int ligneArrivee, int colonneArrivee)
    {
        //Fonction qui regarde si il y a déjà un joueur aux coordonnées indiqués.
        //Si non, le joueur se déplace sur la case.
        //Si oui, le joueur frappe le joueur présent sur la case et lui fait perdre des points de vie
        //Avant que le joueur se déplace ou frappe un autre joueur, on lance un dé

        bool presenceJoueur = false; //Booléen qui renseigne si une personne est déjà sur la case

        for (int i = 0; i < plateau.Joueurs.Count(); i++)
        { //On regarde les positions de chaque joueurs présents sur le tableau
            if (plateau.Joueurs[i].Ligne == ligneArrivee && plateau.Joueurs[i].Colonne == colonneArrivee && plateau.Joueurs[i].Mort == false)
            { //On regarde si un joueur est présent sur la case de notre joueur  
                if (this.EquipeTroll == plateau.Joueurs[i].EquipeTroll || Balle == true)
                {//Si le joueur adjacent au notre est de la même équipe, ou que notre joueur possède la balle
                 //notre joueur se déplace de manière aléatoire

                    //On lance le dé pour savoir si notre joueur réussit à se déplacer
                    bool reussite = ReussirLancerDe(PROBA_DE_SE_DEPLACER);
                    if (reussite == false) //Si le joueur n'arrive pas à se déplacer, il reste sur place et perd un pv
                    {
                        if (EquipeTroll == true)
                            TexteJoueur += $"'(T_T)'!. {Prenom} tombe, il a perdu 1 point de vie. :-D\n";
                        else
                            TexteJoueur += $"'Aïe !'. {Prenom} tombe, il a perdu 1 points de vie. :-( \n";
                        PerdrePV(1);
                    }
                    else
                        DeplacementAleatoire(plateau);
                    presenceJoueur = true;
                }

                else
                {
                    //Si les joueurs ne sont pas de la même équipe et que le joueur n'a pas le ballon, ils se tapent
                    //Si le joueur est une brute ce code ne sera pas effectuer mais contenu dans sa fonction Deplacer
                    presenceJoueur = true;

                    //On lance le dé pour savoir si l'action réussit
                    bool reussite = ReussirLancerDe(PROBA_DE_FRAPPER_NORMALE);
                    if (reussite == true)
                    {
                        if (EquipeTroll == true)
                        { //Les phrases de dialogue sont différentes pour les humains et les trolls
                            TexteJoueur += $"'w(°z°)w'. {Prenom} frappe {plateau.Joueurs[i].Prenom}.\n";
                            TexteJoueur += $"'Outch!'. {plateau.Joueurs[i].Prenom} a perdu {FORCEDEFRAPPE} points de vie\n";
                        }

                        else
                        {
                            TexteJoueur += $"'Tu ne mangeras pas les poulpes, saleté de troll !'. {Prenom} frappe {plateau.Joueurs[i].Prenom}.\n";
                            TexteJoueur += $"'Bien fait!'. {plateau.Joueurs[i].Prenom} a perdu {FORCEDEFRAPPE} points de vie\n";
                        }
                        plateau.Joueurs[i].PerdrePV(FORCEDEFRAPPE);
                        // if (plateau.Joueurs[i].Mort == true)
                        // {
                        //     if (EquipeTroll == true)
                        //         TexteJoueur += $"Oh non! {plateau.Joueurs[i].Prenom} est mort au combat :'( \n";
                        //     else
                        //         TexteJoueur += $"Ah! Ah! {plateau.Joueurs[i].Prenom}, ce maudit troll est mort. Bien fait pour lui ! o(^v^)o \n";
                        // }

                    }
                }
            }
        }

        if (presenceJoueur == false)
        { //Si il n'y a pas de joueurs sur la case convoitée, on cherche à déplacer la joueur à ses coordonnées
            bool reussite = ReussirLancerDe(PROBA_DE_SE_DEPLACER);
            //On lance un dé pour regarder si le joueur réussit l'action
            if (reussite == false) //Si le joueur n'arrive pas à se déplacer, il reste sur place et perd un pv
            {
                if (EquipeTroll == true)
                    TexteJoueur += $"'(T_T)'!. {Prenom} tombe, il a perdu 1 point de vie. :-D\n";
                else
                    TexteJoueur += $"'Aïe !'. {Prenom} tombe, il a perdu 1 points de vie. :-( \n";
                PerdrePV(1);
            }
            else
            {
                Ligne = ligneArrivee;
                Colonne = colonneArrivee;
            }
        }
    }
    public void DeplacementAleatoire(Plateau plateau)
    {
        int deplacementLigne;
        int deplacementColonne;
        do
        {
            deplacementColonne = rnd.Next(-1, 2); //Le joueur se déplace aléatoirement sur une ligne de +1,0 ou -1
            deplacementLigne = rnd.Next(-1, 2); //Le joueur se déplace aléatoirement sur une ligne de +1,0 ou -1

        }
        while (HorsTerrain(plateau, Ligne + deplacementLigne, Colonne + deplacementColonne) == false || (deplacementColonne == 0 && deplacementLigne == 0));
        //Tant que le déplacement du joueur l'emmène hors terrain ou qu'il reste sur place, on continue la boucle

        Deplacement(plateau, Ligne + deplacementLigne, Colonne + deplacementColonne);
        // Ligne += deplacementLigne;
        // Colonne += deplacementColonne;
    }
    public void DeplacerVersBalle(Plateau plateau)
    {
        //Le joueur n'a pas la balle et se déplace vers la balle
        int ecartLigne = plateau.BallonPlateau.Ligne - Ligne;
        int ecartColonne = plateau.BallonPlateau.Colonne - Colonne;
        int l = Ligne; //Ligne à laquelle on aimerait déplacer le joueur
        int c = Colonne; //Colonne à laquelle on aimerait déplacer le joueur
        if (ecartLigne < 0) //le joueur est plus bas que le ballon
        {
            if (HorsTerrain(plateau, Ligne - 1, Colonne) == true)
            {//si le joueur n'est pas hors terrain en montant, on le fait monter. Sinon, il ne bouge pas de ligne
                l--;

            }
        }
        else if (ecartLigne > 0) //Le joueur est plus haut que la balle
        {
            if (HorsTerrain(plateau, Ligne + 1, Colonne) == true)
            {//si le joueur n'est pas hors terrain en descendant, on le fait descendre. Sinon, il ne bouge pas de ligne
                l++;
            }
        }

        if (ecartColonne < 0) //Le joueur est à gauche du ballon
        {
            if (HorsTerrain(plateau, Ligne, Colonne - 1) == true)
            {//si le joueur n'est pas hors terrain en allant à droite, on le fait aller à droite. Sinon, il ne bouge pas de colonne
                c--;
            }
        }
        else if (ecartColonne > 0) //Le joueur est à gauche du ballon
        {
            if (HorsTerrain(plateau, Ligne, Colonne + 1) == true)
            {//si le joueur n'est pas hors terrain en allant à gauche, on le fait aller à gauche. Sinon, il ne bouge pas de colonne
                c++;
            }
        }
        Deplacement(plateau, l, c);
    }
    public void DeplacerVersBut(Plateau plateau)
    {
        //Le joueur a la balle et se déplace vers les buts
        int ecartLigne = plateau.PlateauVide.GetLength(0) / 2 - Ligne;
        int ecartColonne = plateau.PlateauVide.GetLength(1) - 2 - Colonne;//on regarde l'écart avec le bord droit du plateau
        int l = Ligne; //l est la ligne ou l'on aimerait se déplacer
        int c = Colonne; //c est la colonne ou on aimerait se déplacer
        if (EquipeTroll == false) //Le joueur se déplace vers le but de droite
        {
            ecartColonne = 2 - Colonne; //on regarde l'écart avec le bord gauche du plateau
        }

        if (ecartLigne < 0) //Le joueur est en dessous du milieu des buts
        {
            if (HorsTerrain(plateau, Ligne - 1, Colonne) == true) //si le joueur n'est pas hors terrain en descendant, on le fait descendre. Sinon, il ne bouge pas de ligne
                l--;
        }
        else if (ecartLigne > 0) //Le joueur est plus haut que la balle
        {
            if (HorsTerrain(plateau, Ligne + 1, Colonne) == true)
            {//si le joueur n'est pas hors terrain en descendant, on le fait descendre. Sinon, il ne bouge pas de ligne
                l++;
            }
        }

        if (ecartColonne < 0) //Le joueur est à gauche des buts ou il veut marquer
        {
            if (HorsTerrain(plateau, Ligne, Colonne - 1) == true)
            {//si le joueur n'est pas hors terrain en allant à droite, on le fait aller à droite. Sinon, il ne bouge pas de colonne
                c--;
            }
        }
        else if (ecartColonne > 0) //Le joueur est à gauche du ballon
        {
            if (HorsTerrain(plateau, Ligne, Colonne + 1) == true)
            {//si le joueur n'est pas hors terrain en allant à gauche, on le fait aller à gauche. Sinon, il ne bouge pas de colonne
                c++;
            }
        }
        Deplacement(plateau, l, c);
    }

    // ------------------     PRISE DE LA BALLE   --------------------------------------
    public void AvoirBalle(Plateau plateau, Ballon balle)
    {
        //Fonction pour un joueur qui n'a pas la balle
        //Le joueur essaie de contrôler la balle
        //Si la balle n'est pas dans la case du joueur, le joueur ne peut pas réussir
        //Si le joueur réussit, la variable Balle est égale à true
        if (plateau.BallonPlateau.Colonne == this.Colonne && plateau.BallonPlateau.Ligne == this.Ligne)
        {
            int reussite = rnd.Next(0, 101);
            if (reussite > PROBA_DE_RECUPERER_LA_BALLE)
            {
                if (EquipeTroll == true)
                    TexteJoueur += $"'<@_@>'. {Prenom} n'arrive pas à contrôler la balle.\n";
                else
                    TexteJoueur += $"'Le ballon glisse c'est pas ma faute !'. {Prenom} n'arrive pas à contrôler la balle.\n";
            }
            else
            {
                Balle = true;
                balle.BallonStatique(); //Le ballon est contrôlé par le joueur et n'agit plus de lui-même
                if (EquipeTroll == true)
                    TexteJoueur += $"'o(^U^)o'. {Prenom} controle la balle.\n";
                else
                    TexteJoueur += $"'Je l'ai !'. {Prenom} controle la balle.\n";
            }
        }
        else
        {
            TexteJoueur += $"{Prenom} n'a pas la balle à proximiter, il ne peut pas la récupérer.\n";
        }
    }

    // ------------------     LANCEMENT DE LA BALLE ------------------------------------

    public void Lancer(Plateau plateau, Ballon ballon)
    {
        //Fonction pas fini qui permet de savoir si le joueur réussit à lancer la balle.
        //Le résultat est un booléen égal à true si le joueur réussit à false sinon
        //Si le joueur n'a pas la balle, la fonction retourne false
        if (Balle == true)
        {
            int reussite = rnd.Next(0, 101);
            if (reussite < PROBA_DE_LANCER_LA_BALLE)
            {
                Balle = false;
                if (EquipeTroll == true)
                    TexteJoueur += $"'(-^v^)-'. {Prenom} lance la balle.\n";
                else
                    TexteJoueur += $"'Je compte sur vous!'. {Prenom} lance la balle.\n";
                LancerBallon(plateau, ballon);

            }
            else
            {
                Balle = true;
                if (EquipeTroll == true)
                    TexteJoueur += $"'o(*^@^*)o'. {Prenom} garde la balle sans se déplacer.\n";
                else
                    TexteJoueur += $"'Démarquez vous, enfin ! Il vont me prendre la balle si je la lance!'. {Prenom} garde la balle sans se déplacer.\n";
            }

        }
        else
        { //Texte utile pour le test, afin de savoir effectue une action qu'il n'est pas censé faire
            TexteJoueur += $"{Prenom} ne contrôle pas la balle, il ne peut pas la lancer.\n";
        }
    }

    public void LancementDuBallonVersBut(Plateau plateau, Ballon ballon)
    {
        ballon.Impulsion = true;
        Orientation direction;
        //On détermine la direction dans lequel le ballon va être envoyé
        int distanceMilieu = Ligne - plateau.PlateauJeu.GetLength(0) / 2;
        if (EquipeTroll == true)
        {
            if (distanceMilieu == 0)
                direction = Orientation.Droite;
            else if (distanceMilieu > 0)
                direction = Orientation.HautDroite;
            else
                direction = Orientation.BasDroite;
        }
        else
        {
            if (distanceMilieu == 0)
                direction = Orientation.Gauche;
            else if (distanceMilieu > 0)
                direction = Orientation.HautGauche;
            else
                direction = Orientation.BasGauche;
        }
        //Une fois la direction donnée, on déplace deux fois le ballon. 
        //Si un joueur se trouve sur la trajectoire, il peut réceptionner le ballon s'il réussit un lancer de dé
        DeplacementBallon(plateau, ballon, direction);
    }
    public void LancementDuBallonVersUnJoueur(Plateau plateau, Ballon ballon, Joueur joueur)
    {
        ballon.Impulsion = true;
        Orientation direction;
        //On détermine la direction dans lequel le ballon va être envoyé
        int distanceLigneJoueur = Ligne - joueur.Ligne;
        int distanceColonneJoueur = Colonne - joueur.Colonne;
        if (distanceColonneJoueur == 0)
        {
            if (distanceLigneJoueur < 0) //Le joueur est en bas
                direction = Orientation.Bas;
            else //Le joueur est en haut
                direction = Orientation.Haut;
        }
        else if (distanceLigneJoueur == 0)
        {
            if (distanceColonneJoueur < 0) //Le joueur est à droite
                direction = Orientation.Droite;
            else //Le joueur est à gauche
                direction = Orientation.Gauche;
        }
        else if (distanceLigneJoueur < 0 && distanceColonneJoueur < 0)
            direction = Orientation.BasDroite;
        else if (distanceLigneJoueur < 0 && distanceColonneJoueur > 0)
            direction = Orientation.BasGauche;
        else if (distanceLigneJoueur > 0 && distanceColonneJoueur < 0)
            direction = Orientation.HautDroite;
        else
            direction = Orientation.HautGauche;

        //Une fois la direction donnée, on déplace deux fois le ballon. 
        //Si un joueur se trouve sur la trajectoire, il peut réceptionner le ballon s'il réussit un lancer de dé
        DeplacementBallon(plateau, ballon, direction);
    }

    public void LancementDuBallonAleatoire(Plateau plateau, Ballon ballon)
    {
        ballon.Impulsion = true;
        int aleatoire = rnd.Next(1, 9);
        Orientation direction = Orientation.Bas;
        //On choisit la direction aléatoirement
        switch (aleatoire)
        {
            case 1:
                direction = Orientation.Bas;
                break;
            case 2:
                direction = Orientation.Haut;
                break;
            case 3:
                direction = Orientation.Droite;
                break;
            case 4:
                direction = Orientation.Gauche;
                break;
            case 5:
                direction = Orientation.BasDroite;
                break;
            case 6:
                direction = Orientation.BasGauche;
                break;
            case 7:
                direction = Orientation.HautDroite;
                break;
            case 8:
                direction = Orientation.HautGauche;
                break;
        }
        //On déplace le ballon
        DeplacementBallon(plateau, ballon, direction);
    }
    public void DeplacementBallon(Plateau plateau, Ballon ballon, Orientation direction)
    { //Cette fonction déplace le ballon
      //Si le ballon arrive sur un joueur, celui ci a une probabilité de l'arrêter
        if (ballon.Impulsion == true)
            ballon.ModifierPosition(direction); //Le ballon se déplace
        bool prendreBalle = false;
        foreach (Joueur joueur in plateau.Joueurs)
        { //On regarde si le ballon est sur la même position qu'un joueur (autre que le lanceur)
            if (ballon.Colonne == joueur.Colonne && ballon.Ligne == joueur.Ligne && Numero != joueur.Numero && ballon.Impulsion == true && joueur.Mort == false)
            {
                if (joueur.ClasseJoueur() == "brute")
                    prendreBalle = ReussirLancerDe(PROBA_RECEPTION_BALLON_BRUTE);
                else if (joueur.ClasseJoueur() == "gardien")
                    prendreBalle = ReussirLancerDe(PROBA_RECEPTION_BALLON_GARDIEN);
                else if (joueur.ClasseJoueur() == "coureur")
                    prendreBalle = ReussirLancerDe(PROBA_RECEPTION_BALLON_COUREUR);
                else if (joueur.ClasseJoueur() == "defenseur")
                    prendreBalle = ReussirLancerDe(PROBA_RECEPTION_BALLON_DEFENSEUR);

                if (prendreBalle == true)
                {
                    joueur.Balle = true;
                    ballon.BallonStatique();
                    TexteJoueur += $"{joueur.Prenom} intercepte la balle.\n";
                }
                else
                {
                    TexteJoueur += $"{joueur.Prenom} voit la balle passer sous ses jambes.\n";
                }
            }

        }

    }

    // ------------------     DESCRIPTION D'UN JOUEUR ------------------------------------
    public void ReinitialiserTexteJoueur()
    {
        TexteJoueur = "";
    }

    public override string ToString()
    {
        //Retourne une descirption du joueur
        string texte = "";
        if (Balle == true)
        {
            texte = "et contrôle la balle";
        }
        else
            texte = "et ne contrôle pas la balle";
        return $"{Prenom} est en en position à la ligne {Ligne} et à la colonne {Colonne}. \n{Prenom} a {PointDeVie} points de vie " + texte;
    }
}
