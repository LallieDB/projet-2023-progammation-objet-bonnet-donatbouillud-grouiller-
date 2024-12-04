public class FileTools
{   // Classe trouvée et légèrement modifiée pour permettre le stockage d'informations dans un fichier texte

    public FileTools()
    {
    }

    public List<string> Lire(string chemin_fichier)
    {
        //Retourne le contenu d'un fichier sous forme d'une liste de chaine de caractère
        //En argument, il faut rentrer le chemin du fichier à lire
        List<string> Chaine = new List<string>();
        string? line = null;
        FileStream monFlux = new FileStream(chemin_fichier, FileMode.OpenOrCreate);
        StreamReader monLecteur = new StreamReader(monFlux);
        while ((line = monLecteur.ReadLine()) != null)
        {
            Chaine.Add(line); //La chaine "\r\n" créé un retour à la ligne
        }
        monLecteur.Close();
        monFlux.Close();
        return Chaine;
    }

    public void Ecrire(string chemin_fichier, string Chaine)
    {

        //Ecrit le contenu d'une chaine de caracteres dans un fichier
        //L'argument est le chemin du fichier dans lequel on souhaite écrire
        //Ici, suppression d'un fichier de même nom
        //(Ceci evite la levée de l'exception "File already exist")
        if (File.Exists(chemin_fichier))
            File.Delete(chemin_fichier);

        FileStream monFlux = new FileStream(chemin_fichier, FileMode.OpenOrCreate);
        StreamWriter monScribe = new StreamWriter(monFlux);
        monScribe.Write(Chaine);
        monScribe.Close();
        monFlux.Close();
    }


}
