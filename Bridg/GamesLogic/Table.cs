using System;
using System.Collections.Generic;
using System.Linq;


namespace Bridg
{
    public delegate void MessageDelegate(string message);
    public delegate int GetCountDelegate();
    public delegate void ShowCardDelegate(int number, int x, int y);
    public delegate void ShowScoresDelegate(string[] names, string[] scores, int numberSelectPlayer);

    public class Table
    {
        #region Приватні поля

        /// <summary>
        /// Кінець гри
        /// </summary>
        private bool _gameOver;
        /// <summary>
        /// Кінець роздачі
        /// </summary>
        private bool _isDistributionOver;            
        /// <summary>
        /// Всі карти, колода та відкрита колода відповідно
        /// </summary>
        private List<Card> _cards, _deck, _openDeck;               
        /// <summary>
        /// Гравці
        /// </summary>
        private List<Player> _players;                               
        /// <summary>
        /// Множник на який множаться всі очки в кінці одного розіграшу
        /// </summary>
        private int _multiplier;                                 
        /// <summary>
        /// Карта з якою відбувається анімація
        /// </summary>
        private Card _movingCard;
        /// <summary>
        /// Вибраний гравець, наступний гравець та гравець що ходить відповідно
        /// </summary>
        private Player _selectPlayer, _nextSelectPlayer, _globalSelectedPlayer;  

        private int _quantityPlayers;                                   //Кількість гравців
        /// <summary>
        /// вказує на гравця який отримає карти за покладену 8
        /// </summary>
        private Player _takengTwoCards;                         
        /// <summary>
        /// Вибрана масть яку необхідно класти на вальта
        /// </summary>
        private int _suitOfJeck;                                

        private bool _activeChoice;
        /// <summary>
        /// Натиснута карта
        /// </summary>
        private Card _selectedCard;
        /// <summary>
        /// Вказує чи натиснута ліва кнопка миші
        /// </summary>
        private bool _isTakenCard;
        /// <summary>
        /// Вказує чи була вже покладена перша карта
        /// </summary>
        private bool _isSecondMove;
        /// <summary>
        /// Значення яке вказує чи була вже взята гравцем карта
        /// </summary>
        private bool _isToCard;
        /// <summary>
        /// Індекс натиснутої карти
        /// </summary>
        private int _indexSelectedCard;

        private double _dx, _dy;
        /// <summary>
        /// Координати зсуву курсиву мишки від лівого правого кутка
        /// </summary>
        private double _zx, _zy;
        /// <summary>
        /// Величина поля
        /// </summary>
        private int _fildSize;
        /// <summary>
        /// Ширина гральної карти
        /// </summary>
        private int _cardWidth;
        private Place _placeOpenDeck, _placeDeck;
        /// <summary>
        /// Швидкість анімації
        /// </summary>
        private int _speedAnimation;
        #endregion 

        #region Властивості
        /// <summary>
        /// Повертає верхню карту з відкритої колоди. Якщо колода пуста повертає null
        /// </summary>
        public Card OpenDeck
        {
            get
            {
                if (_openDeck.Count > 0)
                    return _openDeck[CountCardsInOpenDeck - 1];
                return null;
            }
        }

        /// <summary>
        /// Властивість для отримання множника
        /// </summary>
        public int Multiplier { get { return _multiplier; } }

        /// <summary>
        /// Номер вибраного гравця
        /// </summary>
        public int SelectedPlayerNumber { get { return _selectPlayer.Number; } }
        /// <summary>
        /// Кількість карт в відкритій колоді
        /// </summary>
        public int CountCardsInOpenDeck { get { return _openDeck.Count; } }
        /// <summary>
        /// Номер масті яку вибрав гравець поклавши валет
        /// </summary>
        public int SuitOfJeck { get { return _suitOfJeck; } }

        public bool ActiveChoice { get { return _activeChoice; } }

        /// <summary>
        /// Вказує чи можна взяти гравцю карту
        /// </summary>
        public bool IsToCard { get { return _isToCard; } }
        /// <summary>
        /// Вказує ширину гральної карти в пікселях
        /// </summary>
        public int CardWidth { get { return _cardWidth; } }
        /// <summary>
        /// Вказує висоту гральної карти
        /// </summary>
        public int CardHeight { get { return (int)(_cardWidth * 1.41); } }

        /// <summary>
        /// Вказує чи був закінчений розклад
        /// </summary>
        public bool IsDistributionOver { get { return _isDistributionOver; } }
        /// <summary>
        /// Швидкість анімації може мати значення від 0 до 100
        /// </summary>
        public int SpeedAnimation 
        {
            set
            {
                if (value >= 0 && value < 100)
                    _speedAnimation = value;
            }
            get { return _speedAnimation; }
        }

        #endregion

        #region Події
        /// <summary>
        /// Подія для відображення повідомлень
        /// </summary>
        public event MessageDelegate ShowMessage;
        /// <summary>
        /// Подія яка повинна надати гравцю можливість
        /// обрати кількість гравців в новій грі
        /// </summary>
        public event GetCountDelegate GetCountPlayers;
        /// <summary>
        /// Подія яка мусить надати користувачу можливість вибрати
        /// масть в разі якщо він(вона) поклав(ла) валета
        /// </summary>
        public event GetCountDelegate GetSuid;
        /// <summary>
        /// Подія з за допомогою якої буде відображатися карта
        /// На гральному столі
        /// </summary>
        public event ShowCardDelegate ShowCard;
        public event ShowScoresDelegate ShowScoresOfPlayers;

        #endregion

        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        public Table(int size)
        {
            this._fildSize = size;
            this._cardWidth = (int)(size * 0.15);

            int y = (int)((size / 2) - (CardHeight / 2));
            _placeDeck = new Place((int)((size / 2) - (_cardWidth * 1.2)), y, false);
            _placeOpenDeck = new Place((int)((size / 2) + (_cardWidth * 0.2)), y, false);

            _speedAnimation = 85;
            NewGame();
        }

        #region Внутрішні функції

        /// <summary>
        /// Ініціалізцє поля початковими значеннями
        /// </summary>
        private void Initialize()
        {
            //Колоди карт
            _cards = new List<Card>();
            _deck = new List<Card>();
            _openDeck = new List<Card>();

            //Приватні поля
            _globalSelectedPlayer = null;
            _gameOver = false;
            _isTakenCard = false;

            if (GetCountPlayers != null)
                _quantityPlayers = GetCountPlayers();
            else _quantityPlayers = 2;

            //Створення нових гравців
            _players = new List<Player>();

            //Заповнення нових карт
            for (int i = 0; i < 36; i++)
            {
                _cards.Add(new Card(i + 1));
            }

            List<Place> places = CalcPlace(_quantityPlayers);
            //Заповнення нових гравців
            for (int i = 0; i < _quantityPlayers; i++)
            {
                _players.Add(new Player(i + 1, places[i]));
                _players[i].score = 0;
                _players[i].Name = "Гравець " + (i + 1).ToString();
            }
        }
        /// <summary>
        /// Розраховує координати для кожного гравця та повертає список місць
        /// </summary>
        /// <param name="count">Кількість гравців для якої потрібно розрахувати місця</param>
        /// <returns>Повертає місце для кожного гравця розпочинаючи з нижнього і рухаючись за годинниковою стрілкою</returns>
        private List<Place> CalcPlace(int count)
        {
            double x1 = 0.025, x2 = 0.25, x3 = 0.825;
            double y1 = 0.76, y2 = 0.15, y3 = 0.03;
            //Місця за годинниковою стрілкою з нижнього гравця
            // player0(x2,y1) player1(x1, y2) player2(x2,y3) player3(x3,y2)

            List<Place> res = new List<Place>();

            res.Add(new Place( (int)(x2*_fildSize), (int)(y1*_fildSize) , true) );
            //Якщо гравці два то посадити їх одне на проти одного
            if (count == 2)
            {
                res.Add(new Place( (int)(x2 * _fildSize), (int)(y3 * _fildSize), true ));
            }
            //Інакше садити всіх за годинниковою стрілкою від від нульового гравця
            else
            {
                res.Add(new Place((int)(x1 * _fildSize), (int)(y2 * _fildSize), false));
                res.Add(new Place((int)(x2 * _fildSize), (int)(y3 * _fildSize), true));

                if (count == 4)
                    res.Add(new Place((int)(x3 * _fildSize), (int)(y2 * _fildSize), false));
            }
            return res;
        }

        /// <summary>
        /// Відправляє всі карти в закриту колоду після чого тасує її та роздає карти для нового розкладу
        /// </summary>
        private void NewDistribution()
        {
            _isDistributionOver = false;
            //numbJacks = 0;
            _multiplier = 1; //Обнулення множника

            ChooseGlobalPlayer();

            _isToCard = true;
            _activeChoice = false;

            SetPlayersRole();

            DropCards();

            for (int i = 0; i < 36; i++)
            {
                _deck.Add(_cards[i]);
            }
            ShuffleCards(_deck);

            PlayOut();

            ReactionOnCards(_openDeck[0]);

            if (_openDeck[_openDeck.Count - 1].Priority() != 6)
                _isSecondMove = true;
            else _isSecondMove = false;
            if (_openDeck[_openDeck.Count - 1].Priority() == 11 && _selectPlayer.Number == 1)
                _activeChoice = true;

            Referi();
        }

        /// <summary>
        /// Очищення карт з усіх колод та з усіх гравців
        /// </summary>
        private void DropCards()
        {
            for (int i = 0; i < _players.Count(); i++)
            {
                _players[i].ClearCards();
            }

            _deck.Clear();
            _openDeck.Clear();
        }

        /// <summary>
        /// Рокласти карти гравцям та викласти одну в відкриту калоду
        /// </summary>
        private void PlayOut()
        {
            for (int i = 0; i < _players.Count() * 5; i++)
            {
                if (_players[i % _players.Count()] != _globalSelectedPlayer)
                {
                    _players[i % _players.Count()].PushCard(_deck[_deck.Count - 1]);
                    _deck.RemoveAt(_deck.Count - 1);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                _globalSelectedPlayer.PushCard(_deck[_deck.Count - 1]);
                _deck.RemoveAt(_deck.Count - 1);
            }

            PutCardInOpenDeck(_deck, _deck.Count - 1);

            //players[0].SortCards();
        }

        /// <summary>
        /// Вибір гравця який буде першим у розкладі
        /// </summary>
        private void ChooseGlobalPlayer()
        {
            if (_globalSelectedPlayer == null)
                _globalSelectedPlayer = _players[0];
            else
            {
                if (_globalSelectedPlayer.Number < _players.Count())
                    _globalSelectedPlayer = _players[_globalSelectedPlayer.Number];
                else
                    _globalSelectedPlayer = _players[0];
            }
        }

        /// <summary>
        /// Призначення гравцям ролей
        /// </summary>
        private void SetPlayersRole()
        {
            _selectPlayer = _globalSelectedPlayer;
            if (_selectPlayer.Number < _players.Count())
            {
                _nextSelectPlayer = _players[_selectPlayer.Number];
                _takengTwoCards = _players[_selectPlayer.Number];
            }
            else
            {
                _nextSelectPlayer = _players[0];
                _takengTwoCards = _players[0];
            }
        }
        /// <summary>
        /// Перевіряє чи знаходиться хоч якась частина card1 над card2
        /// </summary>
        /// <param name="card1">Перша карта</param>
        /// <param name="card2">Друга карта над якою повинна бити перша</param>
        /// <returns>Повертає true якщо перша карта перетинає другу</returns>
        private bool IsCrossedCards(Card card1, Card card2)
        {
            int X1 = (int)card1.X;
            int Y1 = (int)card1.Y;

            //Координати всіх кутків карти починаючи з верхнього лівого і рухаючись за годинниковою стрілкою
            int[] x = new int[4] { X1, X1 + _cardWidth, X1 + _cardWidth, X1 };
            int[] y = new int[4] { Y1, Y1, Y1 + CardHeight, Y1 + CardHeight };

            for (int w = 0; w < 4; w++)
                if (card2.X < x[w] && x[w] < card2.X + _cardWidth &&
                    card2.Y < y[w] && y[w] < card2.Y + CardHeight)
                    return true;
            return false;
        }

        /// <summary>
        /// Потасувати карти в заданому наборі кард
        /// </summary>
        /// <param name="cards">Набір який потрібно потасувати</param>
        private void ShuffleCards(List<Card> cards)
        {
            for (int i = 0; i < 100; i++)
            {
                Random newR = new Random(DateTime.Today.Millisecond * i);
                int n1 = newR.Next(cards.Count);
                Card newC = cards[n1];
                newR = new Random(DateTime.Now.Millisecond * i * i);
                int n2 = newR.Next(cards.Count);
                cards[n1] = cards[n2];
                cards[n2] = newC;
            }
        }

        /// <summary>
        /// Покласти карту в відкриту колоду з заданого списку карт заданий проміжок
        /// </summary>
        /// <param name="card1">Список з якого буде взято карту</param>
        /// <param name="numb">Номер карти яку потрібно взяти</param>
        private void PutCardInOpenDeck(List<Card> card1, int numb)
        {
            if (numb < card1.Count)
            {
                _openDeck.Add(card1[numb]);

                card1.RemoveAt(numb);
            }
        }
        /// <summary>
        /// Додавання гравцю карту з закритої колоди з використанням анімації
        /// </summary>
        /// <param name="player">Гравець якому буде передана карта</param>
        private void PutCardToPlayer(Player player)
        {
            FlipDeck();
            //int speed = 80;

            if (_selectPlayer.Number != 1)
            {
                _deck[_deck.Count - 1].Animation = true;

                Animation(_deck[_deck.Count - 1], player.Place.X, player.Place.Y, false);
                _deck[_deck.Count - 1].Animation = false;
            }

            player.PushCard(_deck[_deck.Count - 1]);
            _deck.RemoveAt(_deck.Count - 1);
        }

        /// <summary>
        /// Перевіряє чи можливий хід після чого робить його відповідною картою відповідного гравця
        /// </summary>
        /// <param name="cards"> Карта якою ходить гравець</param>
        /// <param name="player"> Гравець який ходить</param>
        /// <returns>Повертає true в разі успішності ходу інакше false</returns>
        private bool Move(Card cards, Player player)
        {

            if (!_isSecondMove &&                                                     //Якщо це перша карта за хід
                (((cards.Suid() == _openDeck[CountCardsInOpenDeck - 1].Suid() ||         //Якщо це така ж карта по масті 
                cards.Priority() == _openDeck[CountCardsInOpenDeck - 1].Priority() ||      //Або це така ж карту за пріорітетом
                cards.Priority() == 11) &&                                      //Або ж ця карта валет
                _openDeck[CountCardsInOpenDeck - 1].Priority() != 11) ||                     //И якщо остання карта в колоді не валет
                // Якщо ж валет то перевірити чи відповідає загадіній раніше карті іншим гравцем
                ((cards.Priority() == 11 || cards.Suid() == _suitOfJeck) && _openDeck[CountCardsInOpenDeck - 1].Priority() == 11))
            )
            {
                if (cards.Priority() != 6)
                    _isSecondMove = true;
                //if (cards.Priority() == 11)
                //    numbJacks++;

                if (cards.Priority() == 11 && _selectPlayer.Number == 1)
                    _activeChoice = true;

                _openDeck.Add(cards);
                //openDeck[HeadOpDeck] = cards;
                //headOpDeck++;
                for (int i = 0; i < player.Cards.Count; i++)
                {
                    if (player[i] == cards)
                    {
                        player.PopCard(i);
                        break;
                    }
                }

                ReactionOnCards(cards);
                _isToCard = true;
                return true;
            }


            return false;
        }
        /// <summary>
        /// Перевіряє чи можливо докласти ще одну карту після того як була покладена карта за заданий хід
        /// </summary>
        /// <param name="cards">Карта яку хоче покласти гравець</param>
        /// <param name="player">Гравець який хоче покласти карту</param>
        /// <returns>Повертає true в разі успішності ходу інакше false</returns>
        private bool SecondMove(Card cards, Player player)
        {
            if (_isSecondMove && cards.Priority() == _openDeck[CountCardsInOpenDeck - 1].Priority())
            {
                //if (cards.Priority() == 11)
                //    numbJacks++;
                _openDeck.Add(cards);

                //openDeck[HeadOpDeck] = cards;
                //headOpDeck++;
                for (int i = 0; i < player.Cards.Count; i++)
                {
                    if (player[i] == cards)
                    {
                        player.PopCard(i);
                    }
                }
                ReactionOnCards(cards);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Робить відповідну реакцію на покладену карту
        /// </summary>
        /// <param name="card">Карта на яку потрібно зробити реакцію</param>
        private void ReactionOnCards(Card card)
        {
            //Якщо 7 то наступний гравець бере одну карту
            if (card.Priority() == 7)
            {
                PutCardToPlayer(PopNextPlayer(1));
            }
            //Якщо 8 то наступний гравець бере 2 карти та пропускає хід після чого наступним стає гравець що йде після нього
            if (card.Priority() == 8)
            {
                PutCardToPlayer(_takengTwoCards);
                PutCardToPlayer(_takengTwoCards);
                NextTakengTwoCard();
            }
            //Якщо туз то наступним активним гравцем той що йде після теперішнього
            if (card.Priority() == 1)
            {
                NextSelectdPlayer();
            }
            //Якщо дама пікова додати 5 карт
            if (card.NumberOfCard == 17)
            {
                for (int i = 0; i < 5; i++)
                {
                    PutCardToPlayer(_nextSelectPlayer);
                }
            }
        }

        /// <summary>
        /// Запускає процес штучного інтелекту для гравця який ходить
        /// </summary>
        private void StartAI()
        {
            _selectPlayer.OpenDeckC = _openDeck[CountCardsInOpenDeck - 1];
            _selectPlayer.SuidOfJeck = _suitOfJeck;
            _selectPlayer.TwoWalk = _isSecondMove;
            Player newP = AI(_selectPlayer);

            if (newP.Walks.Count == 0 && (!IsToCard || _openDeck[CountCardsInOpenDeck - 1].Priority() == 6))
            {
                PutCardToPlayer(_selectPlayer);
                newP = AI(_selectPlayer);
                _isToCard = true;
            }

            for (int i = 0; i < newP.Walks.Count; i++)
            {
                _selectPlayer[newP.Walks[i]].Animation = true;
                Animation(_selectPlayer[newP.Walks[i]], _placeOpenDeck.X, _placeOpenDeck.Y, true);
                _selectPlayer[newP.Walks[i]].Animation = false;
                SecondMove(_selectPlayer[newP.Walks[i]], _selectPlayer);
                Move(_selectPlayer[newP.Walks[i]], _selectPlayer);
            }
            if (newP.Walks.Count != 0 && _openDeck[CountCardsInOpenDeck - 1].Priority() == 11)
            {
                _suitOfJeck = AISuid(_selectPlayer);
            }

            NextPlayer();
        }
        private Player AI(Player Pl)
        {
            List<Player> res = new List<Player>();
            Player B = new Player(Pl);
            for (int i = 0; i < B.Cards.Count; i++)
            {
                if (B.Walking(i))
                {
                    res.Add(AI(B));
                    B = new Player(Pl);
                }
                if (i + 1 == B.Cards.Count && res.Count == 0)
                {
                    return B;
                }
            }

            if (res.Count == 0)
            {
                return B;
            }

            Player res2 = res[0];
            for (int i = 1; i < res.Count; i++)
            {
                if (res[i].NumberOfPoints(1) < res2.NumberOfPoints(1))
                    res2 = res[i];
            }

            return res2;
        }
        private int AISuid(Player pl)
        {
            int[] suid = new int[4];
            for (int i = 0; i < pl.Cards.Count; i++)
                suid[pl[i].Suid()]++;
            int res = 0;
            for (int i = 1; i < 4; i++)
                if (suid[res] < suid[i])
                {
                    res = i;
                }
            return res;
        }
        /// <summary>
        /// Робить анімований рух карти в задану точку
        /// </summary>
        /// <param name="card">Карта яка повинна рухатися</param>
        /// <param name="Tx">X координата в яку повинна рухатись карта</param>
        /// <param name="Ty">Y координата в яку повинна рухатися карта</param>
        /// <param name="speed">Швидкість з якою карта повинна рухатися</param>
        /// <param name="open">Ознака відкритості карти яка буде рухатися</param>
        private void Animation(Card card, double Tx, double Ty, bool open)
        {
            _movingCard = card;
            _movingCard.open = open;
            double Mx = Tx - card.X;
            double My = Ty - card.Y;
            Mx /= 50;
            My /= 50;
            int speed2 = 100 - _speedAnimation;
            for (int i = 0; i < 50; i++)
            {
                card.X += Mx;
                card.Y += My;
                System.Threading.Thread.Sleep(speed2);
            }
            _movingCard = null;
        }

        /// <summary>
        /// Закінчення одного розіграшу та підрахунок очків
        /// </summary>
        private void DistributionOver()
        {
            for (int i = 0; i < _players.Count(); i++)
            {
                if (_players[i].Cards.Count == 0)
                {
                    _isDistributionOver = true;

                    //players[i].score -= numbJacks * 20;
                    string message = "Розклад закінчений \n ";

                    foreach (Player tempPlayer in _players)
                    {
                        int quantityPoints = tempPlayer.NumberOfPoints(_multiplier);

                        message += "\n " + tempPlayer.Name + " - " + quantityPoints.ToString();
                    }

                    //MessageBox.Show(messeg);
                    if (ShowMessage != null)
                        ShowMessage(message);

                    _isDistributionOver = false;
                    for (int b = 0; b < _players.Count(); b++)
                    {
                        _players[b].AddPoints(_multiplier);
                    }
                    DeparturePlayer();
                    NewDistribution();
                }
            }
        }
        /// <summary>
        /// Звільнення столу від гравців які перевищили шкалу очків в 125
        /// </summary>
        private void DeparturePlayer()
        {
            for (int i = _players.Count() - 1; i >= 0; i--)
            {
                if (_players[i].score > 125)
                {
                    if (i == 0)
                    {
                        if (ShowMessage != null)
                            ShowMessage("Ви програли!");
                        _gameOver = true;
                        return;
                    }
                    _players.RemoveAt(i);
                    if (_players.Count == 1)
                    {
                        if (ShowMessage != null)
                            ShowMessage("Ви програли");
                        _gameOver = true;
                        return;
                    }
                }

                if (i < _players.Count && _players[i].score == 125)
                {
                    _players[i].score = 0;
                }
            }
        }

        /// <summary>
        /// Якщо закрита колода пуста то відбувається передача всіх карт 
        /// окрім верхньої в закриту колоду та перетасування закритої колоди
        /// </summary>
        private void FlipDeck()
        {
            if (_deck.Count == 0)
            {
                Card tempCard = _openDeck[_openDeck.Count - 1];
                for (int i = 0; i < CountCardsInOpenDeck - 1; i++)
                {
                    _deck.Add(_openDeck[i]);
                }

                _openDeck.Clear();
                _openDeck.Add(tempCard);

                ShuffleCards(_deck);
                _multiplier++;
            }
        }

        /// <summary>
        /// Отримання місце наступного гравця від теперішнього вибраного
        /// </summary>
        /// <param name="quantity">Кількість гравців</param>
        /// <returns>Повертається наступний гравець</returns>
        private Player PopNextPlayer(int quantity)
        {
            Player res = _selectPlayer;
            for (int i = 0; i < quantity; i++)
            {
                if (res.Number >= _players.Count())
                {
                    res = _players[0];
                }
                else
                {
                    res = _players[_selectPlayer.Number];
                }
            }
            return res;
        }

        /// <summary>
        /// Визначає наступного гравця який буде ходити та передає йому хід
        /// </summary>
        private void NextSelectdPlayer()
        {
            if (!_gameOver)
            {
                if (_players.IndexOf(_nextSelectPlayer) >= _players.Count() - 1)
                {
                    _nextSelectPlayer = _players[0];
                }
                else
                {
                    _nextSelectPlayer = _players[_nextSelectPlayer.Number];
                }
            }
        }

        /// <summary>
        /// Вибирає гравця якому буде передано 2 карти якщо заданий гравець покладе 8
        /// </summary>
        private void NextTakengTwoCard()
        {
            bool f = false;
            if (_takengTwoCards.Number == _players.Count())
            {
                _takengTwoCards = _players[0];
                _nextSelectPlayer = _players[0];
                f = true;
            }
            if (_takengTwoCards.Number == _players.Count() - 1 && !f)
            {
                _takengTwoCards = _players[_players.Count() - 1];
                _nextSelectPlayer = _players[_players.Count() - 1];
                f = true;
            }
            if (_takengTwoCards.Number < _players.Count() - 1 && !f)
            {
                _takengTwoCards = _players[_takengTwoCards.Number];
                _nextSelectPlayer = _players[_takengTwoCards.Number - 1];
            }
            if (_takengTwoCards == _selectPlayer)
            {
                NextTakengTwoCard();
                _nextSelectPlayer = _selectPlayer;
            }
        }

        #endregion

        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        #region Функції інтерфейси

        /// <summary>
        /// Розпочинає нову гру
        /// </summary>
        public void NewGame()
        {
            Initialize();

            _suitOfJeck = 0;
            _selectPlayer = _players[0];
            _nextSelectPlayer = _players[1];
            _takengTwoCards = _players[1];
            NewDistribution();
        }

        /// <summary>
        /// Розставляє всі карти в відповідності до того в якій колоді чи в якого гравця вони знаходяться
        /// </summary>
        public void Referi()
        {
            //Якщо гра закінчена річого не робити
            if (_gameOver)
                return;
            //Розстановка карт закритої колоди
            foreach (var tempCard in _deck)
            {
                if (tempCard != _movingCard)
                {
                    tempCard.X = _placeDeck.X;
                    tempCard.Y = _placeDeck.Y;
                    tempCard.open = false;
                }
            }
            //Розстановка карт відкритої колоди
            foreach (var tempCard in _openDeck)
            {
                tempCard.X = _placeOpenDeck.X;
                tempCard.Y = _placeOpenDeck.Y;
                tempCard.open = true;
            }
            //Розстановка карт всіх гравців
            foreach (Player tempPlayer in _players)
            {
                bool open = false;
                if (tempPlayer.Number == 1 || IsDistributionOver)
                    open = true;
                tempPlayer.SetPlaceCards(_cardWidth, open);
            }
            //Виставлення амімованій карті вдповідне місце
            if (_isTakenCard)
            {
                _selectedCard.X = _dx - _zx;
                _selectedCard.Y = _dy - _zy;
            }
        }
        /// <summary>
        /// Відображає всі карти.
        /// Логіка відображення карти повинна бути 
        /// виконана окремо та прив'язана з за
        /// допомогою події ShowCard
        /// </summary>
        public void ShowAllCards()
        {
            try
            {
                if (ShowCard != null && !_gameOver)
                {
                    foreach (var tempPlayer in _players)
                    {
                        foreach (var tempCard in tempPlayer.Cards)
                        {
                            int numberCard = 0;
                            if (tempCard.open)
                                numberCard = tempCard.NumberOfCard;

                            ShowCard(numberCard, (int)tempCard.X, (int)tempCard.Y);

                        }
                    }
                    //Закрита колода
                    if (_deck.Count > 0)
                        ShowCard(0, (int)_deck[0].X, (int)_deck[0].Y);
                    //Відкрита колода
                    if (OpenDeck != null)
                        ShowCard(OpenDeck.NumberOfCard, (int)OpenDeck.X, (int)OpenDeck.Y);
                    //Анімована карта
                    if (_movingCard != null)
                    {
                        int numberCard = 0;
                        if (_movingCard.open)
                            numberCard = _movingCard.NumberOfCard;

                        ShowCard(numberCard, (int)_movingCard.X, (int)_movingCard.Y);
                    }
                    //Натиснута карта
                    if (_isTakenCard)
                        ShowCard(_selectedCard.NumberOfCard, (int)_selectedCard.X, (int)_selectedCard.Y);
                }
            }
            catch (Exception)
            { }
        }
        public void ShowAllScores()
        {
            string[] names = new string[4];
            string[] scores = new string[4];
            int w = 0;
            for (; w < _players.Count; w++)
            {
                names[w] = _players[w].Name;
                scores[w] = _players[w].score.ToString();
            }
            for (; w < 4; w++)
            {
                names[w] = "";
                scores[w] = "";
            }
            ShowScoresOfPlayers(names, scores, _selectPlayer.Number - 1);
        }
        /// <summary>
        /// Натиснення на стіл в заданих координатах
        /// </summary>
        /// <param name="x">Координата ширини</param>
        /// <param name="y">Координата висоти рухаючись зверху вниз</param>
        public void MousDown(double x, double y)
        {
            if (x > 0 && y > 0 && !_gameOver && _selectPlayer.Number == 1 && !IsDistributionOver)
            {
                // Перша карта починається з координат place гравця 0, відстань між картами 1/4 ширини карти, 
                int xRight = (int)(_players[0].Place.X + ( (0.25 * _cardWidth) * _players[0].Cards.Count) );

                if (_players[0].Place.X <= x && x < xRight + (_cardWidth * 0.75) && 
                    _players[0].Place.Y <= y && y < _players[0].Place.Y + CardHeight)
                {
                    //якщо вибрано карту в межах 1/4 то вибрати її
                    if (x < xRight)
                    {
                        _indexSelectedCard = (int)((x - _players[0].Place.X) / (_cardWidth * 0.25));
                        _zx = (x - _players[0].Place.X) % (_cardWidth * 0.25);

                    }
                    // якщо натиснуто останню карту за межею 25 точок але в її межах то вибарти її
                    else
                    {
                        _indexSelectedCard = _players[0].Cards.Count - 1;
                        _zx = x - (_players[0].Place.X + (0.25 * _cardWidth * (_players[0].Cards.Count - 1)) );
                    }


                    _zy = y - _players[0].Place.Y;

                    _selectedCard = _players[0][_indexSelectedCard];
                    _isTakenCard = true;
                }
            }
        }
        public void MousMove(double x, double y)
        {
            _dx = x; _dy = y;
        }
        /// <summary>
        /// Функція для відпускання кнопки миші
        /// </summary>
        /// <param name="x">Координата x в якій була відпущена кнопка</param>
        /// <param name="y">Координата y в якій була відпущена кнопка</param>
        public void MousUp(double x, double y)
        {
            if (_isTakenCard && 
                IsCrossedCards(_selectedCard, _openDeck[_openDeck.Count - 1]) &&
                !_gameOver)
            {
                SecondMove(_selectedCard, _players[0]);
                Move(_selectedCard, _players[0]);
            }
            _isTakenCard = false;
        }

        /// <summary>
        /// Взяти карту якщо це можливо
        /// </summary>
        public void ToCards()
        {
            if (((!IsToCard && !_isSecondMove) || _openDeck[CountCardsInOpenDeck - 1].Priority() == 6) &&
                !_gameOver && !_isDistributionOver)
            {
                PutCardToPlayer(_players[0]);
                _isToCard = true;
            }
        }
        public void NextPlayer()
        {
            if (!IsToCard && _selectPlayer.Number == 1 && ShowMessage != null)
            {
                ShowMessage("Щоб пропустити хід, візьміть карту!");
            }
            if (_openDeck[CountCardsInOpenDeck - 1].Priority() == 6 && _selectPlayer.Number == 1 && ShowMessage != null)
            {
                ShowMessage("Необхідно накрити шістку, якщо не має підходящої карти візьміть ще одну!");
            }
            //Якщо не лежить 6 та покладена карта
            if (_openDeck[CountCardsInOpenDeck - 1].Priority() != 6 && IsToCard && !_gameOver)
            {
                //if (ActivChooce)
                //    ChooiseSuidJack();
                _activeChoice = false;
                _selectPlayer = _nextSelectPlayer;
                NextSelectdPlayer();
                _takengTwoCards = _nextSelectPlayer;
                _isSecondMove = false;
                _isToCard = false;
                DistributionOver();
                //numbJacks = 0;
            }
            if (_selectPlayer.Number != 1 && !_gameOver)
            {
                StartAI();
            }
        }
        /// <summary>
        /// Виводить гравцю вікно з вибором масті яку потрібно вибрати в разі якщо покладений валет
        /// </summary>
        public void ChooiseSuidJack()
        {
            if (_selectPlayer.Number == 1 && _openDeck[CountCardsInOpenDeck - 1].Priority() == 11)
            {        
                _suitOfJeck = GetSuid();
            }
        }
        public void GameOver()
        {

        }

        #endregion

    }
}
