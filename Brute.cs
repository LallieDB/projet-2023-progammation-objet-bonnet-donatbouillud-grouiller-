public class Brute : Joueur
{

    public bool Endormi { get; private set; }

    // --------------------------- CONSTRUCTEUR ---------------------------------------------
    public Brute(string prenom, int ligne, int colonne, int pointdevie, bool EquipeTroll) : base(prenom, ligne, colonne, pointdevie, EquipeTroll)
    {
        Endormi = false;
    }

    // --------------------------- DEPLACEMENT ---------------------------------------------
    public override void Deplacer(Plateau plateau, Ballon ballon)
    {
        //Si la brute n'a pas le ballon, elle se déplace vers le joueur adverse le plus proche
        if (Balle == false)
        {
            bool joueurAdverseVivant = false;
            int numJoueurLePlusProche = -1;
            int distance = plateau.Colonne + plateau.Ligne;
            int i = 0;

            foreach (Joueur joueur in plateau.Joueurs)
            {
                if (joueur.EquipeTroll != EquipeTroll && joueur.Mort == false)
                { //On regarde s'il existe des adversaires vivants sur le plateau
                    joueurAdverseVivant = true;
                    Endormi = false;
                    if (distance >= Math.Abs(joueur.Colonne - Colonne) + Math.Abs(joueur.Ligne - Ligne))
                    { //Si le joueur est plus proche que les précédents, on le choisit comme joueur le plus proche
                        distance = Math.Abs(joueur.Colonne - Colonne) + Math.Abs(joueur.Ligne - Ligne);
                        numJoueurLePlusProche = i;
                    }
                }
                i += 1;
            }
            //La brute à différents comportements selon s'ils existent des joueurs vivants ou non

            if (joueurAdverseVivant == true)
            {
                DeplacerversJoueurEtLeTaper(plateau, plateau.Joueurs[numJoueurLePlusProche]);
            }
            else
            { //S'il ne reste aucun adversaire, la brute s'endort
                Console.WriteLine($"{Prenom} s'endort car il ne peut plus taper personne.");
                Endormi = true;
            }
        }

        else
            Lancer(plateau, ballon);

    }
    public void DeplacerversJoueurEtLeTaper(Plateau plateau, Joueur joueur)
    {
        //Le joueur se déplace vers un autre joueur
        int ecartLigne = joueur.Ligne - Ligne;
        int ecartColonne = joueur.Colonne - Colonne;
        bool bouger = false; //Prend en compte si le troll peut se déplacer ou non
        int l = Ligne; //Ligne à laquelle on aimerait déplacer le joueur
        int c = Colonne; //Colonne à laquelle on aimerait déplacer le joueur
        //La brute ne se déplace pas en diagonale. Elle se déplace d'abord sur la même colonne
        //que le joueur puis sur la même ligne.
        if (ecartColonne < 0) //la brute est à droite que l'autre joueur
        {
            if (HorsTerrain(plateau, Ligne, Colonne - 1) == true)
            {//si le joueur n'est pas hors terrain en allant à droite, on le déplace. Sinon, il ne bouge pas de ligne
                c--;
                bouger = true;
            }
        }
        else if (ecartColonne > 0) //Le joueur est à gauche du joueur
        {
            if (HorsTerrain(plateau, Ligne, Colonne + 1) == true)
            {//si le joueur n'est pas hors terrain en allant à gauche, on le fait aller à gauche. Sinon, il ne bouge pas de colonne
                c++;
                bouger = true;
            }
        }
        if (ecartLigne < 0 && bouger == false) //Le joueur est à droite du joueur
        {
            if (HorsTerrain(plateau, Ligne, Colonne - 1) == true)
            {//si le joueur n'est pas hors terrain en allant à droite, on le fait aller à droite. Sinon, il ne bouge pas de colonne
                c--;
                bouger = true;
            }
        }

        else if (ecartLigne > 0 && bouger == false) //Le joueur est plus haut que l'autre joueur
        {
            if (HorsTerrain(plateau, Ligne + 1, Colonne) == true)
            {//si le joueur n'est pas hors terrain en descendant, on le fait descendre. Sinon, il ne bouge pas de ligne
                l++;
                bouger = true;
            }
        }

        //On a les coordonnées de déplacement, on regarde si le joueur est à ses coordonnées
        if (l == joueur.Ligne && c == joueur.Colonne)
        { // Si le joueur est à proximité de la brute, elle tape le joueur
            bool reussite = ReussirLancerDe(PROBA_DE_FRAPPER_BRUTE);  //On lance le dé
            if (reussite == true)
            {
                joueur.PerdrePV(3 * FORCEDEFRAPPE); //La brute tape 3 fois plus fort que les autres joueurs
                if (EquipeTroll == true)
                    TexteJoueur += $"'Quelle horreur!'. {Prenom} tape de toute ses forces {joueur.Prenom}. \n";
                else
                    TexteJoueur += $"'Quelle superbe droite !'. {Prenom} tape de toute ses forces {joueur.Prenom}. \n";
            }
        }
        else
        { //Si le joueur que le défenseur veut poursuivre n'est pas sur la case,
          //on déplace le joueur aux bonnes coordonnées sauf si un joueur allié si trouve déjà
            Deplacement(plateau, l, c);
        }
    }

    // --------------------------- LANCER LE BALLON ---------------------------------------------
    public override void LancerBallon(Plateau plateau, Ballon ballon)
    {
        //La brute n'est pas intéressée par le match, elle lance aléatoirement le ballon
        LancementDuBallonAleatoire(plateau, ballon);
    }

    // --------------------------- DESCRIPTION ---------------------------------------------
    public override string ClasseJoueur()
    {
        return "brute";
    }


}