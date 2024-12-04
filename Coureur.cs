public class Coureur : Joueur
{
    // --------------------------- CONSTRUCTEUR ---------------------------------------------
    public Coureur(string prenom, int ligne, int colonne, int pointdevie, bool EquipeTroll) : base(prenom, ligne, colonne, pointdevie, EquipeTroll)
    {
        this.EquipeTroll = EquipeTroll;
    }
    // --------------------------- DEPLACEMENT ---------------------------------------------
    public override void Deplacer(Plateau plateau, Ballon ballon)
    { //Fonction qui gère les déplacements d'un joueur. 
      //Un joueur qui n'a pas la balle va vers le ballon. Si il se déplace sur la balle, il peut essayer de la contrôler
      //Si la balle est à la même case que le joueur, il essaye de la contrôler avant d'aller au but
      //S'il contôle déjà la balle, il va vers les buts. 
      //Si le joueur est proche des buts, il tire pour marquer un but

        if (Balle == true)
        { //Si le joueur a la balle il va vers les buts
            if ((EquipeTroll == false && Colonne <= plateau.PlateauJeu.GetLength(1) / 4) || (EquipeTroll == true && Colonne >= 3 * plateau.PlateauJeu.GetLength(1) / 4))
                Lancer(plateau, ballon);
            else
            {
                DeplacerVersBut(plateau);
                ballon.ChangerCoordonneesBallon(Ligne, Colonne);
            }

        }

        else if (Colonne == plateau.BallonPlateau.Colonne && Ligne == plateau.BallonPlateau.Ligne)
        {
            AvoirBalle(plateau, ballon);
            if (Balle == true) //Si le joueur vient de contrôler la balle, il se déplace vers les buts
                DeplacerVersBut(plateau);
        }

        else
        {
            DeplacerVersBalle(plateau);
            if (Colonne == plateau.BallonPlateau.Colonne && Ligne == plateau.BallonPlateau.Ligne)
                AvoirBalle(plateau, ballon); //Si le joueur se déplace sur la case de la balle, il essaye de la contrôler
        }
    }

    // --------------------------- DESCRIPTION ---------------------------------------------
    public override string ClasseJoueur()
    {
        return "coureur";
    }
}