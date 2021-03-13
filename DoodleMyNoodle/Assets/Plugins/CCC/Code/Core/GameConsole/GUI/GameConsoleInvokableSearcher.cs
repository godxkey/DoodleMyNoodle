using System.Collections.Generic;
using UnityEngineX;

public class GameConsoleInvokableSearcher
{
    public bool FilterDisabled { get; set; } = false;
    public bool ReverseResult { get; set; } = false;

    private List<char> _candidateBuffer = new List<char>();
    private List<(int score, IGameConsoleInvokable command)> _suggestionsAndScores = new List<(int score, IGameConsoleInvokable command)>();

    public void GetSuggestions(IEnumerable<IGameConsoleInvokable> searchList, string searchText, List<IGameConsoleInvokable> result)
    {
        _suggestionsAndScores.Clear();
        searchText = searchText.ToLower();

        foreach (var invokable in searchList)
        {
            if (FilterDisabled && !invokable.Enabled)
                continue;

            int score = GetScore(searchText, ((GameConsoleInvokable)invokable).Name);
            if (score > 0)
                _suggestionsAndScores.Add((score, invokable));
        }
        
        if (ReverseResult)
        {
            _suggestionsAndScores.Sort((a, b) =>
            {
                if (a.score == b.score)
                {
                    return b.command.DisplayName.Length.CompareTo(a.command.DisplayName.Length);
                }
                return a.score.CompareTo(b.score);
            });
        }
        else
        {
            _suggestionsAndScores.Sort((a, b) =>
            {
                if (a.score == b.score)
                {
                    return a.command.DisplayName.Length.CompareTo(b.command.DisplayName.Length);
                }
                return b.score.CompareTo(a.score);
            });
        }


        result.Clear();

        for (int i = 0; i < _suggestionsAndScores.Count; i++)
        {
            result.Add(_suggestionsAndScores[i].command);
        }
    }

    private int GetScore(string text, string candidate)
    {
        _candidateBuffer.Clear();

        // add candidate characters to a buffer
        for (int i = 0; i < candidate.Length; i++)
        {
            _candidateBuffer.Add(candidate[i]);
        }

        int index = -1;
        int score = 1; // if the candidate passes the test, it has a starting score of 1

        // for each character in 'text'
        for (int i = 0; i < text.Length; i++)
        {
            int previousIndex = index;

            // if any of the characters match
            index = indexOfFrom(_candidateBuffer, text[i], previousIndex);
            if (index == -1)
            {
                score -= 2;
            }
            else
            {
                // if we matched at the first letter, score ++
                // This is to make 'hello' more favorable to 'push' if submitting 'h'
                if (i == 0 && index == 0)
                {
                    score++;
                }

                // if the character is next to the previous one, score ++
                // This is to make 'hello' more favorable to 'hogward' if submitting 'he'
                if (previousIndex == index)
                {
                    score++;
                }

                _candidateBuffer.RemoveAt(index);
            }
        }

        return score;

        // this method is like list.IndexOf(value), but starts from a specific given position
        int indexOfFrom(List<char> buffer, char c, int fromIndex)
        {
            if (fromIndex < 0)
                fromIndex = 0;

            for (int i = fromIndex; i < buffer.Count; i++)
            {
                if (buffer[i] == c)
                    return i;
            }

            for (int i = 0; i < fromIndex; i++)
            {
                if (buffer[i] == c)
                    return i;
            }
            return -1;
        }
    }
}
