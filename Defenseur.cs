public class Defenseur : Joueur
{
    // Le défenseur est un joueur qui reste vers les buts. 
    // Si un joueur adverse s'approche avec le ballon, il peut lui piquer si il est dans une case adjacente à lui

    // --------------------------- CONSTRUCTEUR ---------------------------------------------
    public Defenseur(string prenom, int ligne, int colonne, int pointdevie, bool EquipeTroll) : base(prenom, ligne, colonne, pointdevie, EquipeTroll)
    {
    }

    // --------------------------- DEPLACEMENT ---------------------------------------------
    public override void Deplacer(Plateau plateau, Ballon ballon)
    {
        //Fonction qui détermine le déplacement d'un défenseur
        //Pour savoir le comportement du joueur, on regarde si il a la balle ou pas
        if (Balle == false) //Comportement si le joueur n'a pas la balle
        { //Si la balle est aux coordonnées du joueur, il essaie de la contrôler
            if (ballon.Ligne == Ligne && ballon.Colonne == Colonne)
                AvoirBalle(plateau, ballon);
            //Sinon, si un joueur adverse a la balle il se déplace vers lui. Sinon, il ne bouge pas
            bool deplacer = false; //Variable qui sert à savoir si le joueur à effectuer son déplacement
            foreach (Joueur joueur in plateau.Joueurs) //On regarde parmi tous les joueurs si le détenteur de la balle est dans l'autre équipe
            {
                if (EquipeTroll != joueur.EquipeTroll && joueur.Ligne == ballon.Ligne && joueur.Colonne == ballon.Colonne && deplacer == false)
                { //Il se déplace vers le joueur adverse qui a la balle et essaie de lui voler
                    DeplacerversJoueurEtPrendreBalle(plateau, joueur, ballon);
                    deplacer = true;
                }
            }
        }
        else
            //Si le joueur à la balle, il la lance
            Lancer(plateau, ballon);

    }
    public void DeplacerversJoueurEtPrendreBalle(Plateau plateau, Joueur joueur, Ballon ballon)
    {
        //Le joueur se déplace vers un autre joueur
        int ecartLigne = joueur.Ligne - Ligne;
        int ecartColonne = joueur.Colonne - Colonne;
        int l = Ligne; //Ligne à laquelle on aimerait déplacer le joueur
        int c = Colonne; //Colonne à laquelle on aimerait déplacer le joueur
        if (ecartLigne < 0) //le joueur est plus bas que l'autre joueur
        {
            if (HorsTerrain(plateau, Ligne - 1, Colonne) == true)
            {//si le joueur n'est pas hors terrain en montant, on le fait monter. Sinon, il ne bouge pas de ligne
                l--;
            }
        }
        else if (ecartLigne > 0) //Le joueur est plus haut que l'autre joueur
        {
            if (HorsTerrain(plateau, Ligne + 1, Colonne) == true)
            {//si le joueur n'est pas hors terrain en descendant, on le fait descendre. Sinon, il ne bouge pas de ligne
                l++;
            }
        }

        if (ecartColonne < 0) //Le joueur est à droite du joueur
        {
            if (HorsTerrain(plateau, Ligne, Colonne - 1) == true)
            {//si le joueur n'est pas hors terrain en allant à droite, on le fait aller à droite. Sinon, il ne bouge pas de colonne
                c--;
            }
        }
        else if (ecartColonne > 0) //Le joueur est à gauche du joueur
        {
            if (HorsTerrain(plateau, Ligne, Colonne + 1) == true)
            {//si le joueur n'est pas hors terrain en allant à gauche, on le fait aller à gauche. Sinon, il ne bouge pas de colonne
                c++;
            }
        }

        //On a les coordonnées de déplacement, on regarde si le joueur adverse est à ses coordonnées
        if (l == joueur.Ligne && c == joueur.Colonne)
        { // Si le joueur est à une case du défenseur, celui-ci cherche à lui prend la balle
          //On regarde d'abord si le joueur réussit l'action
            bool reussite = ReussirLancerDe(PROBA_DE_RECUPERER_LA_BALLE);
            if (reussite == false) //Si le joueur n'arrive pas à se déplacer, il reste sur place et perd un pv
            {
                if (EquipeTroll == true)
                    TexteJoueur += $"'(T_T)'!. {Prenom} essaye de prendre la balle à {joueur.Prenom} mais échoue. :-D\n";
                else
                    TexteJoueur += $"'Aïe !'. {Prenom} essaye de prendre la balle à {joueur.Prenom} mais échoue. :-( \n";
            }
            else //Si le joueur réussit, il prend la balle à son adversaire
            {
                joueur.Balle = false;
                Balle = true;
                ballon.ChangerCoordonneesBallon(Ligne, Colonne);
                if (EquipeTroll == true)
                    TexteJoueur += $"'Quelle horreur!'. {Prenom} pique la balle à {joueur.Prenom}. \n";
                else
                    TexteJoueur += $"'Quelle défense incroyable !'. {Prenom} pique la balle à {joueur.Prenom}. \n";
            }

        }
        else
        { //Si le joueur que le défenseur veut poursuivre n'est pas sur la case où il souhaite se déplacer,
          //on déplace le joueur aux bonnes coordonnées sauf si un joueur allié si trouve déjà
            Deplacement(plateau, l, c);
        }
    }

    // --------------------------- LANCEMENT DE BALLON  ---------------------------------------------

    public override void LancerBallon(Plateau plateau, Ballon ballon)
    {
        //Le défenseur lance la balle vers son allié le plus proche
        //Si tous ses alliés hors gardien sont morts, il lance la balle vers les buts
        bool joueurEnVie = false;
        int distanceJoueurMin = plateau.PlateauJeu.GetLength(0) + plateau.PlateauJeu.GetLength(1);
        int distanceJoueur;
        int numeroJoueurDansLaListe = 0;
        int numeroJoueurLePlusProche = 0;
        //On définit le joueur allié le plus proche
        foreach (Joueur joueur in plateau.Joueurs)
        {
            if (joueur.EquipeTroll == EquipeTroll && joueur.ClasseJoueur() != "gardien" && joueur.ClasseJoueur() != "defenseur" && joueur.Numero != Numero)
            {
                joueurEnVie = true;
                distanceJoueur = Math.Abs(joueur.Colonne - Colonne) + Math.Abs(joueur.Ligne - Ligne);
                if (distanceJoueur <= distanceJoueurMin)
                {
                    distanceJoueurMin = distanceJoueur;
                    numeroJoueurLePlusProche = numeroJoueurDansLaListe;
                }
            }
            numeroJoueurDansLaListe++;
        }
        //Si il y a au moins un joueur allié (hors gardien et défenseur) en vie, il lance le ballon vers le joueur
        if (joueurEnVie == true)
            LancementDuBallonVersUnJoueur(plateau, ballon, plateau.Joueurs[numeroJoueurLePlusProche]);
        //Sinon il lance vers les buts adverses
        else
            LancementDuBallonVersBut(plateau, ballon);
    }

    // --------------------------- DESCRIPTION ---------------------------------------------
    public override string ClasseJoueur()
    {
        return "defenseur";
    }
}