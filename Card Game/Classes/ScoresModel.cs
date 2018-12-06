using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Card_Game 
{
    class ScoresModel : INotifyPropertyChanged
    {
    private int wins, losses, draws = 0;

    // Get/sets the number of draws
    public int Draws
    {
        get
        {
            return this.draws;
        }
        set
        {
            this.draws = value;
            OnPropertyChanged("Draws");
        }
    }

    // Get/sets the number of losses
    public int Losses
    {
        get { return losses; }
        set
        {
            losses = value;
            OnPropertyChanged("Losses");
        }
    }

    // Get/sets the number of wins
    public int Wins
    {
        get { return wins; }
        set
        {
            wins = value;
            OnPropertyChanged("Wins");
        }
    }

    // Resets the wins, losses, and draws to 0
    public void Reset()
    {
        Wins = 0;
        Losses = 0;
        Draws = 0;
    }

    // propertyChanged property of INotifyPropertyChanged 
    // interface MUST be implemented 
    public event PropertyChangedEventHandler PropertyChanged;

    // OnPropertyChanged property of INotifyPropertyChanged 
    // interface MUST be implemented 
    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}

}

