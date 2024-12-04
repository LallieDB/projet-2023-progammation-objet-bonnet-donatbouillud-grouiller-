public class Inventaire
{
    public int NbBaie { get; protected set; }
    public int NbBaieMagique { get; protected set; }

    // --------------------------- CONSTRUCTEUR ---------------------------------------------
    public Inventaire(int nbBaie, int nbBaieMagique)
    {
        NbBaie = nbBaie;
        NbBaieMagique = nbBaieMagique;
    }

    // --------------------------- UTILISATION D'OBJET ---------------------------------------------
    public void UtiliserBaie(Joueur joueur, int PV)
    {
        //le coach utilise une baie (les critères d'autorisations pour utiliser cette méthode sont déjà vérifiés dans Soigner de Simulation)
        if (joueur.PointDeVie == 0)
            joueur.Revivre();
        joueur.PointDeVie += PV;
        if (joueur.PointDeVie > joueur.PointdeVieInitial) // si, en soignant le joueur, ses points de vie sont supérieur au max autoriser, on limite
        {
            joueur.PointDeVie = joueur.PointdeVieInitial;
        }
        NbBaie -= 1;
    }

    public void UtiliserBaieMagique(Joueur joueur)
    {
        //le coach utilise une baie magique (les critères d'autorisations pour utiliser cette méthode sont déjà vérifiés dans Soigner de Simulation)
        joueur.PointDeVie = joueur.PointdeVieInitial;
        joueur.Revivre();
        NbBaieMagique -= 1;
    }
}