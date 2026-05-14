using System.Drawing;
using System.Windows.Forms;

namespace at3_at9_Converter
{
    public static class DialogService
    {
        public static DialogResult ShowFormatDialog(IWin32Window owner, string title, string promptText, Icon icon)
        {
            return ShowTwoChoiceDialog(owner, title, promptText, icon, "MP3", "WAV");
        }

        public static DialogResult ShowTargetCodecDialog(IWin32Window owner, string title, string promptText, Icon icon)
        {
            return ShowTwoChoiceDialog(owner, title, promptText, icon, "AT9", "AT3");
        }

        private static DialogResult ShowTwoChoiceDialog(IWin32Window owner, string title, string promptText, Icon icon, string leftText, string rightText)
        {
            using (Form form = CreateBaseForm(title))
            using (Label label = CreateLabel(promptText))
            using (Button leftButton = CreateButton(leftText, DialogResult.OK))
            using (Button rightButton = CreateButton(rightText, DialogResult.Cancel))
            using (PictureBox iconBox = CreateIcon(icon))
            {
                Size dialogSize = GetDialogSize(promptText, label.Font);
                LayoutMessage(label, iconBox, dialogSize);
                LayoutTwoButtons(leftButton, rightButton, dialogSize);

                form.ClientSize = dialogSize;
                form.Controls.AddRange(new Control[] { iconBox, label, leftButton, rightButton });
                form.AcceptButton = leftButton;
                form.CancelButton = rightButton;

                return form.ShowDialog(owner);
            }
        }

        public static DialogResult ShowYesNoDialog(IWin32Window owner, string title, string promptText, Icon icon, string yesText, string noText)
        {
            using (Form form = CreateBaseForm(title))
            using (Label label = CreateLabel(promptText))
            using (Button buttonYes = CreateButton(yesText, DialogResult.OK))
            using (Button buttonNo = CreateButton(noText, DialogResult.Cancel))
            using (PictureBox iconBox = CreateIcon(icon))
            {
                Size dialogSize = GetDialogSize(promptText, label.Font);
                LayoutMessage(label, iconBox, dialogSize);
                LayoutTwoButtons(buttonYes, buttonNo, dialogSize);

                form.ClientSize = dialogSize;
                form.Controls.AddRange(new Control[] { iconBox, label, buttonYes, buttonNo });
                form.AcceptButton = buttonYes;
                form.CancelButton = buttonNo;

                return form.ShowDialog(owner);
            }
        }

        public static DialogResult ShowOkDialog(IWin32Window owner, string title, string promptText, Icon icon, string okText)
        {
            using (Form form = CreateBaseForm(title))
            using (Label label = CreateLabel(promptText))
            using (Button buttonOK = CreateButton(okText, DialogResult.OK))
            using (PictureBox iconBox = CreateIcon(icon))
            {
                Size dialogSize = GetDialogSize(promptText, label.Font);
                LayoutMessage(label, iconBox, dialogSize);
                buttonOK.SetBounds((dialogSize.Width - 140) / 2, dialogSize.Height - 40, 140, 23);

                form.ClientSize = dialogSize;
                form.Controls.AddRange(new Control[] { iconBox, label, buttonOK });
                form.AcceptButton = buttonOK;

                return form.ShowDialog(owner);
            }
        }

        public static Size GetDialogSize(string text, Font font, int minWidth = 335, int maxWidth = 650)
        {
            int textMaxWidth = maxWidth - 80;

            Size textSize = TextRenderer.MeasureText(
                text,
                font,
                new Size(textMaxWidth, 0),
                TextFormatFlags.WordBreak
            );

            int width = System.Math.Max(minWidth, textSize.Width + 90);
            int height = System.Math.Max(100, textSize.Height + 95);

            return new Size(width, height);
        }

        private static Form CreateBaseForm(string title)
        {
            return new Form
            {
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };
        }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                ForeColor = Color.DarkRed,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Button CreateButton(string text, DialogResult dialogResult)
        {
            return new Button
            {
                Text = text,
                DialogResult = dialogResult,
                ForeColor = Color.Green,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
        }

        private static PictureBox CreateIcon(Icon icon)
        {
            return new PictureBox
            {
                Image = icon.ToBitmap()
            };
        }

        private static void LayoutMessage(Label label, PictureBox iconBox, Size dialogSize)
        {
            label.SetBounds(60, 15, dialogSize.Width - 75, dialogSize.Height - 75);
            iconBox.SetBounds(15, 15, 35, 35);
        }

        private static void LayoutTwoButtons(Button leftButton, Button rightButton, Size dialogSize)
        {
            leftButton.SetBounds((dialogSize.Width / 2) - 145, dialogSize.Height - 40, 140, 23);
            rightButton.SetBounds((dialogSize.Width / 2) + 5, dialogSize.Height - 40, 140, 23);
        }
    }
}
