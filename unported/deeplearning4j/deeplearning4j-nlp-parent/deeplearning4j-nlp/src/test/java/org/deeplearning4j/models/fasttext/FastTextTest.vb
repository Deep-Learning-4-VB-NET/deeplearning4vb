Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.hamcrest.CoreMatchers.hasItems
import static org.hamcrest.MatcherAssert.assertThat
Imports org.junit.jupiter.api.Assertions

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.models.fasttext



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class FastTextTest extends org.deeplearning4j.BaseDL4JTest
	Public Class FastTextTest
		Inherits BaseDL4JTest



		Private inputFile As File = Resources.asFile("models/fasttext/data/labeled_data.txt")
		Private supModelFile As File = Resources.asFile("models/fasttext/supervised.model.bin")
		Private cbowModelFile As File = Resources.asFile("models/fasttext/cbow.model.bin")
		Private supervisedVectors As File = Resources.asFile("models/fasttext/supervised.model.vec")


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTrainSupervised(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTrainSupervised(ByVal testDir As Path)

			Dim output As File = testDir.toFile()

			Dim fastText As FastText = FastText.builder().supervised(True).inputFile(inputFile.getAbsolutePath()).outputFile(output.getAbsolutePath()).build()
			log.info(vbLf & "Training supervised model ..." & vbLf)
			fastText.fit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTrainSkipgram(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTrainSkipgram(ByVal testDir As Path)

			Dim output As File = testDir.toFile()

			Dim fastText As FastText = FastText.builder().skipgram(True).inputFile(inputFile.getAbsolutePath()).outputFile(output.getAbsolutePath()).build()
			log.info(vbLf & "Training supervised model ..." & vbLf)
			fastText.fit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTrainSkipgramWithBuckets(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTrainSkipgramWithBuckets(ByVal testDir As Path)

			Dim output As File = Files.createTempFile(testDir,"newFile","bin").toFile()

			Dim fastText As FastText = FastText.builder().skipgram(True).bucket(150).inputFile(inputFile.getAbsolutePath()).outputFile(output.getAbsolutePath()).build()
			log.info(vbLf & "Training supervised model ..." & vbLf)
			fastText.fit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTrainCBOW(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTrainCBOW(ByVal testDir As Path)

			Dim output As File = Files.createTempFile(testDir,"newFile","bin").toFile()

			Dim fastText As FastText = FastText.builder().cbow(True).inputFile(inputFile.getAbsolutePath()).outputFile(output.getAbsolutePath()).build()
			log.info(vbLf & "Training supervised model ..." & vbLf)
			fastText.fit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void tesLoadCBOWModel()
		Public Overridable Sub tesLoadCBOWModel()

			Dim fastText As New FastText(cbowModelFile)
			fastText.test(cbowModelFile)

			assertEquals(19, fastText.vocab().numWords())
			assertEquals("enjoy", fastText.vocab().wordAtIndex(fastText.vocab().numWords() - 1))

			Dim expected() As Double = {5.040466203354299E-4, 0.001005030469968915, 2.8882650076411664E-4, -6.413314840756357E-4, -1.78931062691845E-4, -0.0023157168179750443, -0.002215880434960127, 0.00274421414360404, -1.5344757412094623E-4, 4.6274057240225375E-4, -1.4383681991603225E-4, 3.7832374800927937E-4, 2.523412986192852E-4, 0.0018913350068032742, -0.0024741862434893847, -4.976555937901139E-4, 0.0039220210164785385, -0.001781729981303215, -6.010578363202512E-4, -0.00244093406945467, -7.98621098510921E-4, -0.0010007203090935946, -0.001640203408896923, 7.897148607298732E-4, 9.131592814810574E-4, -0.0013367272913455963, -0.0014030139427632093, -7.755287806503475E-4, -4.2878396925516427E-4, 6.912827957421541E-4, -0.0011824817629531026, -0.0036014916840940714, 0.004353308118879795, -7.073904271237552E-5, -9.646290563978255E-4, -0.0031849315855652094, 2.3360115301329643E-4, -2.9103990527801216E-4, -0.0022990566212683916, -0.002393763978034258, -0.001034979010000825, -0.0010725988540798426, 0.0018285386031493545, -0.0013178540393710136, -1.6632364713586867E-4, -1.4665909475297667E-5, 5.445032729767263E-4, 2.999933494720608E-4, -0.0014367225812748075, -0.002345481887459755, 0.001117417006753385, -8.688368834555149E-4, -0.001830018823966384, 0.0013242220738902688, -8.880519890226424E-4, -6.888324278406799E-4, -0.0036394784692674875, 0.002179111586883664, -1.7201311129610986E-4, 0.002365073887631297, 0.002688770182430744, 0.0023955567739903927, 0.001469283364713192, 0.0011803617235273123, 5.871498142369092E-4, -7.099180947989225E-4, 7.518937345594168E-4, -8.599072461947799E-4, -6.600041524507105E-4, -0.002724145073443651, -8.365285466425121E-4, 0.0013173354091122746, 0.001083166105672717, 0.0014539906987920403, -3.1698777456767857E-4, -2.387022686889395E-4, 1.9560157670639455E-4, 0.0020277926232665777, -0.0012741144746541977, -0.0013026101514697075, -1.5212174912448972E-4, 0.0014194383984431624, 0.0012500399025157094, 0.0013362085446715355, 3.692879108712077E-4, 4.319801155361347E-5, 0.0011261265026405454, 0.0017244465416297317, 5.564604725805111E-5, 0.002170475199818611, 0.0014707016525790095, 0.001303741242736578, 0.005553730763494968, -0.0011097051901742816, -0.0013661726843565702, 0.0014100460102781653, 0.0011811562580987811, -6.622733199037611E-4, 7.860265322960913E-4, -9.811905911192298E-4}
			assertArrayEquals(expected, fastText.getWordVector("enjoy"), 2e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPredict()
		Public Overridable Sub testPredict()
			Dim text As String = "I like soccer"

			Dim fastText As New FastText(supModelFile)
			assertEquals(48, fastText.vocab().numWords())
			assertEquals("association", fastText.vocab().wordAtIndex(fastText.vocab().numWords() - 1))

			Dim expected() As Double = {-0.006423053797334433, 0.007660661358386278, 0.006068876478821039, -0.004772625397890806, -0.007143457420170307, -0.007735592778772116, -0.005607823841273785, -0.00836215727031231, 0.0011235733982175589, 2.599214785732329E-4, 0.004131870809942484, 0.007203693501651287, 0.0016768622444942594, 0.008694255724549294, -0.0012487826170399785, -0.00393667770549655, -0.006292815785855055, 0.0049359360709786415, -3.356488887220621E-4, -0.009407570585608482, -0.0026168026961386204, -0.00978928804397583, 0.0032913016621023417, -0.0029464277904480696, -0.008649969473481178, 8.056449587456882E-4, 0.0043088337406516075, -0.008980576880276203, 0.008716211654245853, 0.0073893265798687935, -0.007388216909021139, 0.003814412746578455, -0.005518500227481127, 0.004668557550758123, 0.006603693123906851, 0.003820829326286912, 0.007174000144004822, -0.006393063813447952, -0.0019381389720365405, -0.0046371882781386375, -0.006193376146256924, -0.0036685809027403593, 7.58899434003979E-4, -0.003185075242072344, -0.008330358192324638, 3.3206873922608793E-4, -0.005389622412621975, 0.009706716984510422, 0.0037855932023376226, -0.008665262721478939, -0.0032511046156287193, 4.4134497875347733E-4, -0.008377416990697384, -0.009110655635595322, 0.0019723298028111458, 0.007486093323677778, 0.006400121841579676, 0.00902814231812954, 0.00975200068205595, 0.0060582347214221954, -0.0075621469877660275, 1.0270809434587136E-4, -0.00673140911385417, -0.007316927425563335, 0.009916870854794979, -0.0011407854035496712, -4.502215306274593E-4, -0.007612560410052538, 0.008726916275918484, -3.0280642022262327E-5, 0.005529289599508047, -0.007944817654788494, 0.005593308713287115, 0.003423960180953145, 4.1348213562741876E-4, 0.009524818509817123, -0.0025129399728029966, -0.0030074280221015215, -0.007503866218030453, -0.0028124507516622543, -0.006841592025011778, -2.9375351732596755E-4, 0.007195258513092995, -0.007775942329317331, 3.951996040996164E-4, -0.006887971889227629, 0.0032655203249305487, -0.007975360378623009, -4.840183464693837E-6, 0.004651934839785099, 0.0031739831902086735, 0.004644941072911024, -0.007461248897016048, 0.003057275665923953, 0.008903342299163342, 0.006857945583760738, 0.007567950990051031, 0.001506582135334611, 0.0063307867385447025, 0.005645462777465582}
			assertArrayEquals(expected, fastText.getWordVector("association"), 2e-3)

			Dim label As String = fastText.predict(text)
			assertEquals("__label__soccer", label)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testIllegalState()
		Public Overridable Sub testIllegalState()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim text As String = "I like soccer"
			Dim fastText As New FastText(supModelFile)
			assertEquals(48, fastText.vocab().numWords())
			assertEquals("association", fastText.vocab().wordAtIndex(fastText.vocab().numWords() - 1))
			Dim expected() As Double = {-0.006423053797334433, 0.007660661358386278, 0.006068876478821039, -0.004772625397890806, -0.007143457420170307, -0.007735592778772116, -0.005607823841273785, -0.00836215727031231, 0.0011235733982175589, 2.599214785732329E-4, 0.004131870809942484, 0.007203693501651287, 0.0016768622444942594, 0.008694255724549294, -0.0012487826170399785, -0.00393667770549655, -0.006292815785855055, 0.0049359360709786415, -3.356488887220621E-4, -0.009407570585608482, -0.0026168026961386204, -0.00978928804397583, 0.0032913016621023417, -0.0029464277904480696, -0.008649969473481178, 8.056449587456882E-4, 0.0043088337406516075, -0.008980576880276203, 0.008716211654245853, 0.0073893265798687935, -0.007388216909021139, 0.003814412746578455, -0.005518500227481127, 0.004668557550758123, 0.006603693123906851, 0.003820829326286912, 0.007174000144004822, -0.006393063813447952, -0.0019381389720365405, -0.0046371882781386375, -0.006193376146256924, -0.0036685809027403593, 7.58899434003979E-4, -0.003185075242072344, -0.008330358192324638, 3.3206873922608793E-4, -0.005389622412621975, 0.009706716984510422, 0.0037855932023376226, -0.008665262721478939, -0.0032511046156287193, 4.4134497875347733E-4, -0.008377416990697384, -0.009110655635595322, 0.0019723298028111458, 0.007486093323677778, 0.006400121841579676, 0.00902814231812954, 0.00975200068205595, 0.0060582347214221954, -0.0075621469877660275, 1.0270809434587136E-4, -0.00673140911385417, -0.007316927425563335, 0.009916870854794979, -0.0011407854035496712, -4.502215306274593E-4, -0.007612560410052538, 0.008726916275918484, -3.0280642022262327E-5, 0.005529289599508047, -0.007944817654788494, 0.005593308713287115, 0.003423960180953145, 4.1348213562741876E-4, 0.009524818509817123, -0.0025129399728029966, -0.0030074280221015215, -0.007503866218030453, -0.0028124507516622543, -0.006841592025011778, -2.9375351732596755E-4, 0.007195258513092995, -0.007775942329317331, 3.951996040996164E-4, -0.006887971889227629, 0.0032655203249305487, -0.007975360378623009, -4.840183464693837E-6, 0.004651934839785099, 0.0031739831902086735, 0.004644941072911024, -0.007461248897016048, 0.003057275665923953, 0.008903342299163342, 0.006857945583760738, 0.007567950990051031, 0.001506582135334611, 0.0063307867385447025, 0.005645462777465582}
			assertArrayEquals(expected, fastText.getWordVector("association"), 2e-3)
			Dim label As String = fastText.predict(text)
			fastText.wordsNearest("test",1)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPredictProbability()
		Public Overridable Sub testPredictProbability()
			Dim text As String = "I like soccer"

			Dim fastText As New FastText(supModelFile)

			Dim result As Pair(Of String, Single) = fastText.predictProbability(text)
			assertEquals("__label__soccer", result.First)
			assertEquals(-0.6930, result.Second, 2e-3)

			assertEquals(48, fastText.vocabSize())
			assertEquals(0.0500, fastText.LearningRate, 2e-3)
			assertEquals(100, fastText.Dimension)
			assertEquals(5, fastText.ContextWindowSize)
			assertEquals(5, fastText.Epoch)
			assertEquals(5, fastText.NegativesNumber)
			assertEquals(1, fastText.WordNgrams)
			assertEquals("softmax", fastText.LossName)
			assertEquals("sup", fastText.ModelName)
			assertEquals(0, fastText.NumberOfBuckets)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVocabulary()
		Public Overridable Sub testVocabulary()
			Dim fastText As New FastText(supModelFile)
			assertEquals(48, fastText.vocab().numWords())
			assertEquals(48, fastText.vocabSize())

			Dim expected() As String = {"</s>", ".", "is", "game", "the", "soccer", "?", "football", "3", "12", "takes", "usually", "A", "US", "in", "popular", "most", "hours", "and", "clubs", "minutes", "Do", "you", "like", "Is", "your", "favorite", "games", "Premier", "Soccer", "a", "played", "by", "two", "teams", "of", "eleven", "players", "The", "Football", "League", "an", "English", "professional", "league", "for", "men's", "association"}

			Dim i As Integer = 0
			Do While i < fastText.vocabSize()
				assertEquals(expected(i), fastText.vocab().wordAtIndex(i))
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLoadIterator() throws java.io.FileNotFoundException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLoadIterator()
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile.getAbsolutePath())
			FastText.builder().supervised(True).iterator(iter).build().loadIterator()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testState()
		Public Overridable Sub testState()
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim fastText As New FastText()
			fastText.predict("something")
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPretrainedVectors(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPretrainedVectors(ByVal testDir As Path)
			Dim output As New File(testDir.toFile(),"newfile.bin")
			output.deleteOnExit()
			Dim fastText As FastText = FastText.builder().supervised(True).inputFile(inputFile.getAbsolutePath()).pretrainedVectorsFile(supervisedVectors.getAbsolutePath()).outputFile(output.getAbsolutePath()).build()

			log.info(vbLf & "Training supervised model ..." & vbLf)
			fastText.fit()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Similarities seem arbitrary, needs verification") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testWordsStatistics(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWordsStatistics(ByVal testDir As Path)
			Dim output As File = Files.createTempFile(testDir,"output","bin").toFile()

			Dim fastText As FastText = FastText.builder().supervised(True).inputFile(inputFile.getAbsolutePath()).outputFile(output.getAbsolutePath()).build()

			log.info(vbLf & "Training supervised model ..." & vbLf)
			fastText.fit()

			Dim file As New File(output.getAbsolutePath() & ".vec")
			Dim word2Vec As Word2Vec = WordVectorSerializer.readAsCsv(file)

			assertEquals(48, word2Vec.getVocab().numWords())
			assertEquals(0.12572339177131653, word2Vec.similarity("Football", "teams"), 2e-3)
			assertEquals(-0.10597872734069824, word2Vec.similarity("professional", "minutes"), 2e-3)
			assertEquals(Double.NaN, word2Vec.similarity("java","cpp"), 0.0)
			'assertThat(word2Vec.wordsNearest("association", 3), hasItems("Football", "Soccer", "men's"));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordsNativeStatistics()
		Public Overridable Sub testWordsNativeStatistics()
			Dim fastText As New FastText()
			fastText.loadPretrainedVectors(supervisedVectors)

			log.info(vbLf & "Training supervised model ..." & vbLf)

			assertEquals(48, fastText.vocab().numWords())
			assertThat(fastText.wordsNearest("association", 3), hasItems("most","eleven","hours"))
			assertEquals(0.1657, fastText.similarity("Football", "teams"), 2e-3)
			assertEquals(0.3661, fastText.similarity("professional", "minutes"), 2e-3)
			assertEquals(Double.NaN, fastText.similarity("java","cpp"), 0.0)
		End Sub
	End Class

End Namespace