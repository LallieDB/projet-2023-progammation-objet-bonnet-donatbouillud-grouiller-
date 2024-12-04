public class De
{
    public int? ValeurDe { get; set; }
    public static Random rng = new Random();

    // --------------------------- CONSTRUCTEUR ---------------------------------------------
    public De()
    {
        LancerDe();
    }

    // --------------------------- LANCEMENT DU DE ---------------------------------------------
    public void LancerDe()
    {
        //on lancer un dé de 2
        ValeurDe = rng.Next(1, 3);
    }

    public override string ToString()
    {
        //Affichage avant de commencer une partie afin de déterminer quelle équipe aura l'avantage
        //ie : celle qui aura le ballon le plus proche d'elle

        //premier affichage avant que le dé soit lancé (en vérifté, le dé est lancé dès le début dans lorsque l'on utilise Simuler)
        string affichage = "";
        Console.Clear();

        Console.WriteLine("  Le suspense est insoutenable...\n");
        Console.WriteLine("  |-___-|     ---------    _______");
        Console.WriteLine(" _|     |_    |  ---  |   | o   o |");
        Console.WriteLine("|  (   )  |   | | ? | |   |   ~   |");
        Console.WriteLine("|.   V   .|   |  ---  |   |_  -  _|");
        Console.WriteLine("|   ---   |   ---------     |___|  \n");
        Console.WriteLine("Appuyez sur 'Entrée' pour lancer le dé");
        string reponse = Console.ReadLine()!;

        Console.Clear();
        switch (ValeurDe)
        {
            // affichage si c'est l'équipe 1 qui gagne
            case 1:
                affichage += " L'équipe des Trolls gagne le lancé de dé !\n\n  |-___-|     ---------    _______\n _|     |_    |  ---  |   | x   x |\n";
                affichage += "|  ^   ^  |   | | 1 | |   |   ~   |\n|.   V   .|   |  ---  |   |_  o  _|\n|  |___|  |   ---------     |___|  ";
                break;

            // affichage si c'est l'équipe 2 qui gagne
            case 2:
                affichage += " L'équipe des Etudiants de l'ENSC gagne le lancé de dé !\n\n  |-___-|     ---------    _______\n _|     |_    |  ---  |   | ^   ^ |\n";
                affichage += "|  X   X  |   | | 2 | |   |   ~   |\n|.  _V_  .|   |  ---  |   |_  U  _|\n|  |___|  |   ---------     |___|  ";
                break;
        }
        return affichage;
    }
}