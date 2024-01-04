using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KombP
{
    public partial class Form1 : Form
    {

        public Form1()
        {

            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

            List<int> inputNumbers = textBox1.Text.Split(' ')
                                                    .Select(n => int.TryParse(n, out int result) ? result : (int?)null)
                                                    .Where(n => n.HasValue)
                                                    .Select(n => n.Value)
                                                    .ToList();

            List<int> ReverseinputNumbers = textBox1.Text.Split(' ')
                                                    .Select(n => int.TryParse(n, out int result) ? result : (int?)null)
                                                    .Where(n => n.HasValue)
                                                    .Select(n => n.Value)
                                                    .Reverse() 
                                                    .ToList();

            
            richTextBox2.Clear();
            richTextBox3.Clear();
            richTextBox4.Clear();

            foreach (string line in richTextBox1.Lines)
            {
                if (CheckIfNumbersAreSequential(inputNumbers, line))
                {
                    HighlightAndCopyLine(inputNumbers, line);
                    HighlightAndCopyLine2(inputNumbers, line);
                }

            }

            foreach (string line in richTextBox1.Lines)
            {
                if (CheckIfNumbersAreSequential(ReverseinputNumbers, line))
                {
                    HighlightAndCopyLine3(ReverseinputNumbers, line);
                }
            }

            foreach (string line in richTextBox1.Lines)
            {
                if (!CheckIfNumbersAreSequential(inputNumbers, line) &&
                    CheckIfNumbersAreSequentialWithGaps(inputNumbers, line, out List<int> seqIndices, out List<int> gapIndices))
                {
                    if(gapIndices.Count() <=8)
                    { 
                    HighlightAndCopyLineWithGaps(line, seqIndices, gapIndices);
                    }
                }



            }

            foreach (string line in richTextBox1.Lines)
            {
                if (!CheckIfNumbersAreSequential(ReverseinputNumbers, line) &&
                    CheckIfNumbersAreSequentialWithGaps(ReverseinputNumbers, line, out List<int> seqIndices, out List<int> gapIndices))
                {
                    if (gapIndices.Count() <= 8)
                    {
                        HighlightAndCopyLineWithGaps2(line, seqIndices, gapIndices);
                    }
                }

            }

        }

        private bool CheckIfNumbersAreSequentialWithGaps(List<int> numbers, string line, out List<int> seqIndices, out List<int> gapIndices)
        {
            string[] words = line.Split(' ');
            seqIndices = new List<int>();
            gapIndices = new List<int>();

            int numbersIndex = 0, lastSeqIndex = -1, gapCount = 0;
            bool isGapAllowed = true;

            for (int i = 0; i < words.Length; i++)
            {
                if (int.TryParse(words[i], out int num))
                {
                    if (numbersIndex < numbers.Count && num == numbers[numbersIndex])
                    {
                        if (lastSeqIndex != -1 && i - lastSeqIndex > 1)
                        {
                            if (isGapAllowed)
                            {
                                gapIndices.AddRange(Enumerable.Range(lastSeqIndex + 1, i - lastSeqIndex - 1));
                            }
                            else
                            {
                                return false;
                            }
                            isGapAllowed = false;
                        }

                        seqIndices.Add(i);
                        lastSeqIndex = i;
                        numbersIndex++;
                        gapCount = 0;

                        if (numbersIndex == numbers.Count)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (lastSeqIndex != -1 && i - lastSeqIndex > 1)
                        {
                            gapCount += i - lastSeqIndex - 1;
                        }
                    }
                }
            }

            return false;
        }

        private void HighlightAndCopyLineWithGaps(string line, List<int> seqIndices, List<int> gapIndices)
        {
            richTextBox2.AppendText(line + "\n");
            int startIndex = richTextBox2.Text.LastIndexOf(line);
            string[] words = line.Split(' ');

            foreach (var index in seqIndices)
            {
                HighlightWordInRichTextBox(startIndex, words, index, Color.Red);
            }

            foreach (var index in gapIndices)
            {
                HighlightWordInRichTextBox(startIndex, words, index, Color.Blue);
            }
        }

        private void HighlightWordInRichTextBox(int startIndexOfLine, string[] words, int wordIndex, Color color)
        {
            string word = words[wordIndex];
            int startIndexOfWord = FindWordStartIndexInLine(startIndexOfLine, words, wordIndex);
            richTextBox2.Select(startIndexOfWord, word.Length);
            richTextBox2.SelectionColor = color;
        }

        private void HighlightAndCopyLineWithGaps2(string line, List<int> seqIndicesR, List<int> gapIndicesR)
        {
            richTextBox4.AppendText(line + "\n");
            int startIndex = richTextBox4.Text.LastIndexOf(line);
            string[] words = line.Split(' ');

            foreach (var index in seqIndicesR)
            {
                HighlightWordInRichTextBox2(startIndex, words, index, Color.Red);
            }

            foreach (var index in gapIndicesR)
            {
                HighlightWordInRichTextBox2(startIndex, words, index, Color.Blue);
            }
        }

        private void HighlightWordInRichTextBox2(int startIndexOfLine, string[] words, int wordIndex, Color color)
        {
            string word = words[wordIndex];
            int startIndexOfWord = FindWordStartIndexInLine(startIndexOfLine, words, wordIndex);
            richTextBox4.Select(startIndexOfWord, word.Length);
            richTextBox4.SelectionColor = color;
        }

        private int FindWordStartIndexInLine(int startIndexOfLine, string[] words, int wordIndex)
        {
            int wordPosition = 0;
            for (int i = 0; i < wordIndex; i++)
            {
                wordPosition += words[i].Length + 1;
            }
            return startIndexOfLine + wordPosition;
        }

        private bool CheckIfNumbersAreSequential(List<int> numbers, string line)
        {
            string[] words = line.Split(' ');
            int numbersIndex = 0;
            foreach (var word in words)
            {
                if (int.TryParse(word, out int num) && num == numbers[numbersIndex])
                {
                    numbersIndex++;
                    if (numbersIndex == numbers.Count)
                    {
                        return true;
                    }
                }
                else
                {
                    numbersIndex = 0;
                }
            }
            return false;
        }

        private void HighlightAndCopyLine(List<int> numbers, string line)
        {
            richTextBox2.AppendText(line + "\n");
            int startIndex = richTextBox2.Text.LastIndexOf(line);

            string sequence = string.Join(" ", numbers);

            int sequenceIndex = line.IndexOf(sequence);

            if (sequenceIndex >= 0)
            {
                richTextBox2.Select(startIndex + sequenceIndex, sequence.Length);
                richTextBox2.SelectionColor = Color.Red;
            }
        }

        private void HighlightAndCopyLine2(List<int> numbers, string line)
        {
            richTextBox3.AppendText(line + "\n");
            int startIndex = richTextBox3.Text.LastIndexOf(line);

            string sequence = string.Join(" ", numbers);

            int sequenceIndex = line.IndexOf(sequence);

            if (sequenceIndex >= 0)
            {
                richTextBox3.Select(startIndex + sequenceIndex, sequence.Length);
                richTextBox3.SelectionColor = Color.Red;
            }
        }

        private void HighlightAndCopyLine3(List<int> numbers, string line)
        {
            richTextBox4.AppendText(line + "\n");
            int startIndex = richTextBox4.Text.LastIndexOf(line);

            string sequence = string.Join(" ", numbers);

            int sequenceIndex = line.IndexOf(sequence);

            if (sequenceIndex >= 0)
            {
                richTextBox4.Select(startIndex + sequenceIndex, sequence.Length);
                richTextBox4.SelectionColor = Color.Red;
            }
        }

        private void FixRow()
        {
            string text = richTextBox1.Text;
            StringBuilder formattedText = new StringBuilder();

            int currentIndex = 0;
            while (currentIndex < text.Length)
            {
                while (currentIndex < text.Length && text[currentIndex] == ' ')
                {
                    currentIndex++;
                }

                int length = Math.Min(58, text.Length - currentIndex);
                string line = text.Substring(currentIndex, length);

                if (length == 58 && currentIndex + 58 < text.Length && char.IsDigit(text[currentIndex + 58]))
                {
                    line += text[currentIndex + 58];
                    currentIndex++;

                    if (currentIndex + 59 < text.Length && text.Substring(currentIndex + 58, 2) == "  ")
                    {
                        currentIndex += 2;
                    }
                }

                line = Regex.Replace(line, @"\s+", " ");

                if (line.EndsWith(" "))
                {
                    line = line.TrimEnd();
                }

                formattedText.AppendLine(line);
                currentIndex += length;
            }

            richTextBox1.Text = formattedText.ToString();
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            FixRow();
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (!char.IsControl(e.KeyChar))
            {
                int selectionStart = richTextBox1.SelectionStart;
                int selectionLength = richTextBox1.SelectionLength;

                richTextBox1.SelectedText = e.KeyChar.ToString();

                richTextBox1.SelectionStart = selectionStart + 1;
                richTextBox1.SelectionLength = 0;

                e.Handled = true;
            }
            else if (e.KeyChar == ' ')
            {
                int selectionStart = richTextBox1.SelectionStart;

                richTextBox1.SelectedText = " ";
                richTextBox1.SelectionStart = selectionStart + 1;
                richTextBox1.SelectionLength = 0;

                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            const char Delete = (char)8;
            e.Handled = !Char.IsDigit(e.KeyChar) && e.KeyChar != Delete && e.KeyChar != ' ';
        }
    }
}