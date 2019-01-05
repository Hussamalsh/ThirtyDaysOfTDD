using System;
using System.Linq;

namespace TDDlearn2
{
    public class StringUtils
    {
        public int CharacterCounter(string sentence, char passedChar)
        {
            if (sentence == null)
            {
                throw new InvalidOperationException();
            }

            return sentence.Count(x => x.ToString().ToLower() == passedChar.ToString().ToLower());
        }
    }
}
