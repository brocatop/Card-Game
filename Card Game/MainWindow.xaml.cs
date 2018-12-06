using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CS225CardLibrary;


namespace Card_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Data model
        private ScoresModel scores = new ScoresModel();

        private Deck myDeck;
        private Card card1, card2, midCard;	// vars to store dealt cards
        private int cardNumber, cardsDealt;	// card counters

        // Constants for the display text
        const string WIN_TEXT = "You were right! ";
        const string LOSS_TEXT = "WRONG. You lose. ";
        const string DRAW_TEXT = "It's a draw. ";
        const string DEAL_AGAIN_TEXT = "Hit Deal to play again.";
        const string GUESS_TEXT = "Will the next card be between these two?";


        public MainWindow()
        {
            InitializeComponent();
            // Initialize and shuffle the deck
            myDeck = new Deck();
            myDeck.Shuffle();

            // Set the data context to our ScoresModel
            base.DataContext = scores;

        }
        private void DealThirdCard(String choice)
        {
            // Timer delay: wait 1.2 seconds before comparing the two dealt cards
            DispatcherTimer compTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.6) };
            compTimer.Start();
            compTimer.Tick += (send, args) =>
            {
                compTimer.Stop();
                compareCards(card1, card2, midCard, choice);

                // Update button states
                dealButton.IsEnabled = true;
                yesButton.IsEnabled = false;
                noButton.IsEnabled = false;
            };

            // Deal the middle card; increment cards dealt
            midCard = DealCard(3);
            this.cardsDealt += 1;
        }


        /// <summary>
        /// Paints a card on the GUI
        /// </summary>
        private void PaintCard(ContentControl cc, Card card)
        {
            // Find the rank and suit Labels in cardTemplate
            Label rank = cc.Template.FindName("rankLabel", cc) as Label;
            Label suit = cc.Template.FindName("suitLabel", cc) as Label;

            // Set the labels with the card's rank and suit
            rank.Content = card.GetRankAsString();
            suit.Content = card.GetSuitAsString();

            // If the card is a diamond or heart, paint the rank and suit red
            if (card.suit == Suits.Diamonds || card.suit == Suits.Hearts)
            {
                rank.Foreground = new SolidColorBrush(Colors.Red);
                suit.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        // Deals the @seq (1, 2, 3) Card in the hand
        private Card DealCard(int seq)
        {
            // Create a new ContentControl; apply the cardTemplate to make it look like a playing card
            ContentControl card = new ContentControl();
            card.Template = Resources["cardTemplate"] as ControlTemplate;
            card.ApplyTemplate();

            // Draw a new card from the deck
            Card myCard = myDeck.GetCard(cardNumber++);

            // Paint the rank & suit on the card
            this.PaintCard(card, myCard);

            // Set the starting region of the card animation
            // - The first card moves to the left
            // - The second card moves to the right
            string canvasAnchor = "";
            switch (seq)
            {
                case 1: canvasAnchor = "(Canvas.Left)"; break;
                case 2: canvasAnchor = "(Canvas.Right)"; break;
                default: canvasAnchor = "(Canvas.Left)"; break;
            }
            // Add the card to the GUI
            playArea.Children.Add(card);

            // Set the animation starting and ending points, depending on which card we're dealing
            if (seq == 3)
            {
                AnimateCard(card, playArea.ActualWidth / 2 - 45, playArea.ActualWidth / 2 - 45, canvasAnchor);
                AnimateCard(card, card.ActualHeight + 20, card.ActualHeight + 150, "(Canvas.Bottom)");
            }
            else
            {
                AnimateCard(card, playArea.ActualWidth / 2 - 45, 20, canvasAnchor);
                AnimateCard(card, card.ActualHeight + 20, card.ActualHeight + 100, "(Canvas.Bottom)");
            }

            return myCard;
        }



        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void resetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            scores.Reset();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dealButton_Click(object sender, RoutedEventArgs e)
        {
            // If there are fewer than 3 cards left in the deck, shuffle it
            // and reset the cards in the GUI
            if (cardsDealt >= 50)
            {
                // Remove all dealt cards
                playArea.Children.RemoveRange(0, playArea.Children.Count);

                // Use a new shuffled deck
                myDeck = new Deck();
                myDeck.Shuffle();

                // Reset the card counters
                cardsDealt = 0;
                cardNumber = 0;
            }
            else if (cardsDealt > 0 && midCard != null)
            {
                // Remove the middle card dealt from the previous hand
                playArea.Children.RemoveAt(playArea.Children.Count - 1);
            }

            // Reset Card fields
            card1 = null;
            card2 = null;
            midCard = null;
            // Timer delay: wait 1.2 seconds before comparing the two dealt cards
            DispatcherTimer compTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.2) };
            compTimer.Start();
            compTimer.Tick += (send, args) =>
            {
                compTimer.Stop();
                CompareCards(card1, card2);
            };

            // Timer delay: wait 0.6 seconds before dealing the second card
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            timer.Start();
            timer.Tick += (send, args) =>
            {
                timer.Stop();
                card2 = DealCard(2);
            };

            // Deal the first card
            card1 = DealCard(1);

            // Update the number of cards dealt by 2
            this.cardsDealt += 2;



        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            DealThirdCard("yes");
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            DealThirdCard("no");
        }

        /// <summary>
        /// Animates the given @card ContentControl's @propertyToAnimate, starting 
        /// at the @from value and ending at the @to value.
        /// </summary>
        private void AnimateCard(ContentControl card, double from, double to, string propertyToAnimate)
        {
            // Create a new Storyboard with a one-way animation
            Storyboard storyboard = new Storyboard() { AutoReverse = false };

            // Create a new Animation object, setting its from, to, and duration properties.
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
            };

            // Use the card and animation together on the Storyboard
            Storyboard.SetTarget(animation, card);

            // Set up the animation path using @propertyToAnimate as the guide
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));

            // Add the animation to the Storyboard and start it
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        /// <summary>
        /// Compares two cards, determining the difference between their ranks;
        /// updates the GUI accordingly
        /// </summary>
        /// <param name="card1">The first Card to compare</param>
        /// <param name="card2">The second Card to compare</param>
        private void CompareCards(Card card1, Card card2)
        {
            // If the difference between the 2 ranks is 0 or 1, we have a draw.
            if (card1.rank == card2.rank || Math.Abs(card1.rank - card2.rank) == 1)
            {
                // Update the status label and scores model
                statusLabel.Content = DRAW_TEXT + DEAL_AGAIN_TEXT;
                scores.Draws++;
            }
            else
            {
                // Otherwise, update button states
                dealButton.IsEnabled = false;
                yesButton.IsEnabled = true;
                noButton.IsEnabled = true;

                // Ask for a guess
                statusLabel.Content = GUESS_TEXT;
            }
        }

        /// <summary>
        /// Compare the 3rd card with the other two dealt cards; take into account
        /// the user's choice when scoring.
        /// </summary>
        /// <param name="card1">The first card to compare</param>
        /// <param name="card2">The second card to compare</param>
        /// <param name="midCard">The third card dealt; compare with @card1 & @card2</param>
        /// <param name="choice">The user's choice, YES or NO, as to whether the rank of
        /// @midCard falls between card1's and card2's rank.
        /// </param>
        private void compareCards(Card card1, Card card2, Card midCard, string choice)
        {
            // Create a temp list of all 3 cards & sort them
            List<Card> cards = new List<Card> {
                card1, card2, midCard
            };
            cards.Sort();

            // Was the 3rd card between the other two, and did the user guess correctly?
            if (cards[0].rank < midCard.rank && midCard.rank < cards[2].rank)
            {
                if (choice == "yes")
                {
                    statusLabel.Content = WIN_TEXT + DEAL_AGAIN_TEXT;
                    scores.Wins++;
                }
                else
                {
                    statusLabel.Content = LOSS_TEXT + DEAL_AGAIN_TEXT;
                    scores.Losses++;
                }
            }
            else
            {
                if (choice == "no")
                {
                    statusLabel.Content = WIN_TEXT + DEAL_AGAIN_TEXT;
                    scores.Wins++;
                }
                else
                {
                    statusLabel.Content = LOSS_TEXT + DEAL_AGAIN_TEXT;
                    scores.Losses++;
                }
            }
        }



    }
}
