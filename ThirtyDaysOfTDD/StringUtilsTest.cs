using NUnit.Framework;
using NUnit.Framework.Internal;
using TDDlearn2;
/*
“Create a library method that takes in a sentence and a single character as parameters. 
The method should return a number that indicates how many times the character appears in the sentence.”    
*/
namespace Tests
{
    [TestFixture]
    public class StringUtilsTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase(0,"",'m')]
        [TestCase(0, "hussa", 'm')]
        [TestCase(1, "m", 'm')]
        [TestCase(2, "mm", 'm')]
        [TestCase(2, "huss5a5", '5')]
        [TestCase(1, "5", '5')]
        [TestCase(0, "1234", '5')]
        [TestCase(0, "1234", ' ')]
        [TestCase(2, "TDD is awesome!", 'D')]//
        [TestCase(2, "TDD is awesome!", 'd')]//
        [TestCase(2, "TDD is awesome!", 'E')]//
        [TestCase(5, "Once is unique, twice is a coincidence, three times is a pattern.", 'n')]//
        public void CharacterCounter_SentencePassed_NumberOfOccurncyReturned(int exceptedValue,string passedSentence, char passedChar )
        {
            //Arrange
            StringUtils stringUtils = new StringUtils();            
            
            //Act 
            int returnedValue = stringUtils.CharacterCounter(passedSentence, passedChar);

            //Assert
            Assert.AreEqual(exceptedValue, returnedValue);
        }

        [Test]
        public void CharacterCounter_NullPassed_ErrorReturned()
        {
            //Arrange
            StringUtils stringUtils = new StringUtils();

            //Act 
            //int returnedValue = stringUtils.CharacterCounter(null, 'm');

            //Assert
            //Assert.Throws(() => stringUtils.CharacterCounter(null, 'm'));
            Assert.Throws<System.InvalidOperationException>(() => stringUtils.CharacterCounter(null, 'm'));

        }

        /*[Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldGetAnArgumentExceptionWhenCharacterToScanForIsLargerThanOneCharacter()
        {
            var sentenceToScan = "This test should throw an exception";
            var characterToScanFor = "xx";
            var stringUtils = new StringUtils();

            stringUtils.CharacterCounter(sentenceToScan, characterToScanFor);
        }*/

    }
}