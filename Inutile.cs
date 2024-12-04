public class Inutile : Joueur
{ //Classe utilisé seulement pour les tests. Un joueur inutile ne sert à rien, il ne bouge pas.
  //Cette classe est pratique pour tester la gestion du score des différentes équipes
    public Inutile(string prenom, int ligne, int colonne, int pointdevie, bool EquipeTroll) : base(prenom, ligne, colonne, pointdevie, EquipeTroll)
    {
    }
    public override string ClasseJoueur()
    {
        return "inutile";
    }
    public override void Deplacer(Plateau plateau, Ballon ballon)
    {

    }

}