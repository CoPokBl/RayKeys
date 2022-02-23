using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class Menu {
        public delegate void EscapeEventD();
        public event EscapeEventD EscapeEvent;
        
        public delegate void ChangeSelectionEventD(int beforeId, int afterId);
        public event ChangeSelectionEventD ChangeSelectionEvent;
        
        public List<Page> pages;

        public int CurrentPage { get; private set; }
        public int CurrentSelection { get; private set; }
        public int CurrentId { get; private set; }
        private int setCurrentId = 0; // multiple buttons would press at once stupid
        private Vector2 tPos;

        private List<int> history = new List<int>();
        
        private int currentI;

        public void ChangePageNoHistory(int pageId) {
            tPos = pages[pageId].pos.ToVector2();
            //Console.WriteLine(pageId);

            CurrentPage = pageId;
            ChangeSelection(0);
            CurrentId = -1;
        }

        public void ChangePage(int pageId) {
            history.Add(CurrentPage);
            ChangePageNoHistory(pageId);
        }

        private void PageChangeEventListener(int id, params object[] args) {
            int pageId = (int) args[0];

            if ((bool)args[1]) ChangePage(pageId);
            else      ChangePageNoHistory(pageId);
        }

        private void FunctionCallEventListener(int id, params object[] args) {
            Action action = (Action) args[0];
            action.Invoke();
        }
        
        public Menu() {
            pages = new List<Page>();

            Game1.Game.UpdateEvent += Update;
        }

        private void Update(float delta) {
            CurrentId = setCurrentId;

            RRender.CameraPos.X = ThingTools.Lerp(RRender.CameraPos.X, tPos.X, 10 * delta);
            RRender.CameraPos.Y = ThingTools.Lerp(RRender.CameraPos.Y, tPos.Y, 10 * delta);

            if (RKeyboard.IsKeyPressed(Keys.Escape)) {
                if (history.Count == 0) {
                    EscapeEvent?.Invoke();
                }
                else {
                    ChangePageNoHistory(history[^1]);
                    history.RemoveAt(history.Count - 1);
                }
            }
            
            if (RKeyboard.IsKeyPressed(Keys.Down)) {
                if (CurrentSelection >= pages[CurrentPage].Items.Count - 1) ChangeSelection(0);
                else                                                        ChangeSelection(CurrentSelection + 1);
            }
            else if (RKeyboard.IsKeyPressed(Keys.Up)) {
                if (CurrentSelection <= 0) ChangeSelection(pages[CurrentPage].Items.Count - 1);
                else                       ChangeSelection(CurrentSelection - 1);
            }
        }

        private void ChangeSelection(int selection) {
            CurrentSelection = selection;
            setCurrentId = pages[CurrentPage].Items[CurrentSelection].Id;
            
            ChangeSelectionEvent?.Invoke(CurrentId, setCurrentId);
        }

        public void AddPage(int x, int y) {
            pages.Add(new Page(x, y));
        }

        public Button AddButton(int page, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            x += pages[page].pos.X; y += pages[page].pos.Y;
            
            Button button = new Button(this, h, v, hT, vT, currentI, text, x, y, sizeX, sizeY, fontSize);
            currentI++;

            pages[page].Items.Add(button);
            return button;
        }
        
        public Button AddPageChangeButton(int page, int pageTo, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Button button = AddButton(page, h, v, hT, vT, text, x, y, sizeX, sizeY, fontSize);
            button.ClickEvent += PageChangeEventListener;
            button.args = new object[] {pageTo, true};

            return button;
        }
        
        public Button AddPageChangeNoHistoryButton(int page, int pageTo, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Button button = AddButton(page, h, v, hT, vT, text, x, y, sizeX, sizeY, fontSize);
            button.ClickEvent += PageChangeEventListener;
            button.args = new object[] {pageTo, false};

            return button;
        }
        
        public Button AddFunctionCallButton(int page, Action func, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Button button = AddButton(page, h, v, hT, vT, text, x, y, sizeX, sizeY, fontSize);
            button.ClickEvent += FunctionCallEventListener;
            button.args = new object[] {func};

            return button;
        }
    }
}