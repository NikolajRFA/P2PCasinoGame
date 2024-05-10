using System;
using Newtonsoft.Json;

namespace Xyaneon.Games.Cards.StandardPlayingCards
{
    /// <summary>
    /// A card from a standard 52-card deck.Jabra Connect 5T True Wireless
    /// </summary>
    /// <remarks>
    /// More information about a standard 52-card deck can be found here:
    /// https://en.wikipedia.org/wiki/Standard_52-card_deck
    /// </remarks>
    /// <seealso cref="StandardPlayingCards.Rank"/>
    /// <seealso cref="StandardPlayingCards.Suit"/>
    /// <seealso cref="StandardPlayingCardDeck"/>
    public class StandardPlayingCard : Card, IEquatable<StandardPlayingCard>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="StandardPlayingCard"/> class using the provided rank
        /// and suit.
        /// </summary>
        /// <param name="rank">
        /// The <see cref="StandardPlayingCards.Rank"/> of the card.
        /// </param>
        /// <param name="suit">
        /// The <see cref="StandardPlayingCards.Suit"/> of the card.
        /// </param>
        [JsonConstructor]
        public StandardPlayingCard(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        /// <summary>
        /// Indicates whether the current <see cref="StandardPlayingCard"/>
        /// is equal to another <see cref="StandardPlayingCard"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="StandardPlayingCard"/> to compare with this
        /// object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the other
        /// <see cref="StandardPlayingCard"/> is equal to this
        /// <see cref="StandardPlayingCard"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <seealso cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(StandardPlayingCard other)
        {
            if (other == null)
            {
                return false;
            }

            return Rank.Equals(other.Rank) && Suit.Equals(other.Suit);
        }

        /// <summary>
        /// Gets the <see cref="StandardPlayingCards.Rank"/> of this card.
        /// </summary>
        public Rank Rank { get; }

        /// <summary>
        /// Gets the <see cref="StandardPlayingCards.Suit"/> of this card.
        /// </summary>
        public Suit Suit { get; }

        /// <summary>
        /// Determines whether the specified object is equal to
        /// this <see cref="StandardPlayingCard"/>.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to the current
        /// <see cref="StandardPlayingCard"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified object is equal to
        /// this <see cref="StandardPlayingCard"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as StandardPlayingCard);
        }

        /// <summary>
        /// Gets a hash code for this <see cref="StandardPlayingCard"/>.
        /// </summary>
        /// <returns>
        /// A a hash code for the current <see cref="StandardPlayingCard"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Rank.GetHashCode() ^ Suit.GetHashCode();
        }

        /// <summary>
        /// Determines whether two <see cref="StandardPlayingCard"/>
        /// instances are equal to each other.
        /// </summary>
        /// <param name="card1">
        /// The <see cref="StandardPlayingCard"/> on the left hand of the
        /// expression.
        /// </param>
        /// <param name="card2">
        /// The <see cref="StandardPlayingCard"/> on the right hand of the
        /// expression.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="card1"/> is equal to
        /// <paramref name="card2"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(StandardPlayingCard card1, StandardPlayingCard card2)
        {
            if (ReferenceEquals(card1, card2))
            {
                return true;
            }

            if (card1 is null)
            {
                return false;
            }

            if (card2 is null)
            {
                return false;
            }

            return card1.Equals(card2);
        }

        /// <summary>
        /// Determines whether two <see cref="StandardPlayingCard"/>
        /// instances are not equal to each other.
        /// </summary>
        /// <param name="card1">
        /// The <see cref="StandardPlayingCard"/> on the left hand of the
        /// expression.
        /// </param>
        /// <param name="card2">
        /// The <see cref="StandardPlayingCard"/> on the right hand of the
        /// expression.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="card1"/> is not equal to
        /// <paramref name="card2"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(StandardPlayingCard card1, StandardPlayingCard card2)
        {
            if (ReferenceEquals(card1, card2))
            {
                return false;
            }

            if (card1 is null)
            {
                return true;
            }

            if (card2 is null)
            {
                return true;
            }

            return !card1.Equals(card2);
        }

        public override string ToString()
        {
            string rankDisplay;
            switch (Rank)
            {
                case Rank.Ace:
                    rankDisplay = "A";
                    break;
                case Rank.Queen:
                    rankDisplay = "Q";
                    break;
                case Rank.Jack:
                    rankDisplay = "J";
                    break;
                case Rank.King:
                    rankDisplay = "K";
                    break;
                default:
                    rankDisplay = ((int)Rank).ToString();
                    break;
            }

            /*char suitDisplay = ' ';
            switch (Suit)
            {
                case Suit.Clubs:
                    suitDisplay = SuitSymbols.WhiteClubSuit;
                    break;
                case Suit.Diamonds:
                    suitDisplay = SuitSymbols.WhiteDiamondSuit;
                    break;
                case Suit.Hearts:
                    suitDisplay = SuitSymbols.WhiteHeartSuit;
                    break;
                case Suit.Spades:
                    suitDisplay = SuitSymbols.WhiteSpadeSuit;
                    break;

            }*/


            return $"{rankDisplay} of {Suit}";
        }
    }
}