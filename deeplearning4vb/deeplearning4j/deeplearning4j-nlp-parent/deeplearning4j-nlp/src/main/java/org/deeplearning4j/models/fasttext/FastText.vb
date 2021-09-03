Imports System
Imports System.Collections.Generic
Imports System.IO
Imports JFastText = com.github.jfasttext.JFastText
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NotImplementedException = org.apache.commons.lang3.NotImplementedException
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports org.deeplearning4j.models.embeddings
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports org.deeplearning4j.models.embeddings.reader
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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
'ORIGINAL LINE: @Slf4j @AllArgsConstructor @lombok.Builder public class FastText implements org.deeplearning4j.models.embeddings.wordvectors.WordVectors, Serializable
	<Serializable>
	Public Class FastText
		Implements WordVectors

		Private Const METHOD_NOT_AVAILABLE As String = "This method is available for text (.vec) models only - binary (.bin) model currently loaded"
		' Mandatory
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String inputFile;
		Private inputFile As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String outputFile;
		Private outputFile As String

		' Optional for dictionary
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int bucket = -1;
		Private bucket As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int minCount = -1;
		Private minCount As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int minCountLabel = -1;
		Private minCountLabel As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int wordNgrams = -1;
'JAVA TO VB CONVERTER NOTE: The field wordNgrams was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private wordNgrams_Conflict As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int minNgramLength = -1;
		Private minNgramLength As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int maxNgramLength = -1;
		Private maxNgramLength As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private int samplingThreshold = -1;
		Private samplingThreshold As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field labelPrefix was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labelPrefix_Conflict As String

		' Optional for training
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean supervised;
		Private supervised As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean quantize;
		Private quantize As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean predict;
'JAVA TO VB CONVERTER NOTE: The field predict was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private predict_Conflict As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean predict_prob;
		Private predict_prob As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean skipgram;
		Private skipgram As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean cbow;
		Private cbow As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean nn;
		Private nn As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean analogies;
		Private analogies As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String pretrainedVectorsFile;
		Private pretrainedVectorsFile As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private double learningRate = -1.0;
'JAVA TO VB CONVERTER NOTE: The field learningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private learningRate_Conflict As Double = -1.0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double learningRateUpdate = -1.0;
		Private learningRateUpdate As Double = -1.0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int dim = -1;
		Private [dim] As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int contextWindowSize = -1;
'JAVA TO VB CONVERTER NOTE: The field contextWindowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private contextWindowSize_Conflict As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int epochs = -1;
		Private epochs As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String modelName;
'JAVA TO VB CONVERTER NOTE: The field modelName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private modelName_Conflict As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private String lossName;
'JAVA TO VB CONVERTER NOTE: The field lossName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lossName_Conflict As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int negativeSamples = -1;
		Private negativeSamples As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int numThreads = -1;
		Private numThreads As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean saveOutput = false;
		Private saveOutput As Boolean = False

		' Optional for quantization
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int cutOff = -1;
		Private cutOff As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean retrain;
		Private retrain As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean qnorm;
		Private qnorm As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean qout;
		Private qout As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Builder.@Default private int dsub = -1;
		Private dsub As Integer = -1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.text.sentenceiterator.SentenceIterator iterator;
		Private iterator As SentenceIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private transient com.github.jfasttext.JFastText fastTextImpl = new com.github.jfasttext.JFastText();
		<NonSerialized>
		Private fastTextImpl As New JFastText()
		<NonSerialized>
		Private word2Vec As Word2Vec
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean modelLoaded;
		Private modelLoaded As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean modelVectorsLoaded;
		Private modelVectorsLoaded As Boolean
		Private vocabCache As VocabCache

		Public Sub New(ByVal modelPath As File)
			Me.New()
			loadBinaryModel(modelPath.getAbsolutePath())
		End Sub

		Public Sub New()
			fastTextImpl = New JFastText()
		End Sub

		Private Class ArgsFactory

'JAVA TO VB CONVERTER NOTE: The field args was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend args_Conflict As IList(Of String) = New List(Of String)()

			Friend Overridable Sub add(ByVal label As String, ByVal value As String)
				args_Conflict.Add(label)
				args_Conflict.Add(value)
			End Sub

			Friend Overridable Sub addOptional(ByVal label As String, ByVal value As Integer)
				If value >= 0 Then
					args_Conflict.Add(label)
					args_Conflict.Add(Convert.ToString(value))
				End If
			End Sub

			Friend Overridable Sub addOptional(ByVal label As String, ByVal value As Double)
				If value >= 0.0 Then
					args_Conflict.Add(label)
					args_Conflict.Add(Convert.ToString(value))
				End If
			End Sub

			Friend Overridable Sub addOptional(ByVal label As String, ByVal value As String)
				If StringUtils.isNotEmpty(value) Then
					args_Conflict.Add(label)
					args_Conflict.Add(value)
				End If
			End Sub

			Friend Overridable Sub addOptional(ByVal label As String, ByVal value As Boolean)
				If value Then
					args_Conflict.Add(label)
				End If
			End Sub


			Public Overridable Function args() As String()
				Dim asArray(args_Conflict.Count - 1) As String
				Return args_Conflict.toArray(asArray)
			End Function
		End Class

		Private Function makeArgs() As String()
			Dim argsFactory As New ArgsFactory()

			argsFactory.addOptional("cbow", cbow)
			argsFactory.addOptional("skipgram", skipgram)
			argsFactory.addOptional("supervised", supervised)
			argsFactory.addOptional("quantize", quantize)
			argsFactory.addOptional("predict", predict_Conflict)
			argsFactory.addOptional("predict_prob", predict_prob)

			argsFactory.add("-input", inputFile)
			argsFactory.add("-output", outputFile)

			argsFactory.addOptional("-pretrainedVectors", pretrainedVectorsFile)

			argsFactory.addOptional("-bucket", bucket)
			argsFactory.addOptional("-minCount", minCount)
			argsFactory.addOptional("-minCountLabel", minCountLabel)
			argsFactory.addOptional("-wordNgrams", wordNgrams_Conflict)
			argsFactory.addOptional("-minn", minNgramLength)
			argsFactory.addOptional("-maxn", maxNgramLength)
			argsFactory.addOptional("-t", samplingThreshold)
			argsFactory.addOptional("-label", labelPrefix_Conflict)
			argsFactory.addOptional("analogies",analogies)
			argsFactory.addOptional("-lr", learningRate_Conflict)
			argsFactory.addOptional("-lrUpdateRate", learningRateUpdate)
			argsFactory.addOptional("-dim", [dim])
			argsFactory.addOptional("-ws", contextWindowSize_Conflict)
			argsFactory.addOptional("-epoch", epochs)
			argsFactory.addOptional("-loss", lossName_Conflict)
			argsFactory.addOptional("-neg", negativeSamples)
			argsFactory.addOptional("-thread", numThreads)
			argsFactory.addOptional("-saveOutput", saveOutput)
			argsFactory.addOptional("-cutoff", cutOff)
			argsFactory.addOptional("-retrain", retrain)
			argsFactory.addOptional("-qnorm", qnorm)
			argsFactory.addOptional("-qout", qout)
			argsFactory.addOptional("-dsub", dsub)

			Return argsFactory.args()
		End Function

		Public Overridable Sub fit()
			Dim cmd() As String = makeArgs()
			fastTextImpl.runCmd(cmd)
		End Sub

		Public Overridable Sub loadIterator()
			If iterator IsNot Nothing Then
				Try
					Dim tempFile As File = File.createTempFile("FTX", ".txt")
					Dim writer As New StreamWriter(tempFile)
					Do While iterator.hasNext()
						Dim sentence As String = iterator.nextSentence()
						writer.Write(sentence)
					Loop

					fastTextImpl = New JFastText()
				Catch e As IOException
					log.error(e.Message)
				End Try
			End If
		End Sub

		Public Overridable Sub loadPretrainedVectors(ByVal vectorsFile As File)
			word2Vec = WordVectorSerializer.readWord2VecModel(vectorsFile)
			modelVectorsLoaded = True
			log.info("Loaded vectorized representation from file %s. Functionality will be restricted.", vectorsFile.getAbsolutePath())
		End Sub

		Public Overridable Sub loadBinaryModel(ByVal modelPath As String)
			fastTextImpl.loadModel(modelPath)

			modelLoaded = True
		End Sub

		Public Overridable Sub unloadBinaryModel()
			fastTextImpl.unloadModel()
			modelLoaded = False
		End Sub

		Public Overridable Sub test(ByVal testFile As File)
			fastTextImpl.test(testFile.getAbsolutePath())
		End Sub

		Private Sub assertModelLoaded()
			If Not modelLoaded AndAlso Not modelVectorsLoaded Then
				Throw New System.InvalidOperationException("Model must be loaded before predict!")
			End If
		End Sub

		Public Overridable Function predict(ByVal text As String) As String

			assertModelLoaded()

			Dim label As String = fastTextImpl.predict(text)
			Return label
		End Function

		Public Overridable Function predictProbability(ByVal text As String) As Pair(Of String, Single)

			assertModelLoaded()

			Dim predictedProbLabel As JFastText.ProbLabel = fastTextImpl.predictProba(text)

			Dim retVal As New Pair(Of String, Single)()
			retVal.First = predictedProbLabel.label
			retVal.Second = predictedProbLabel.logProb
			Return retVal
		End Function

		Public Overridable Function vocab() As VocabCache Implements WordVectors.vocab
			If modelVectorsLoaded Then
				vocabCache = word2Vec.vocab()
			Else
				If Not modelLoaded Then
					Throw New System.InvalidOperationException("Load model before calling vocab()")
				End If

				If vocabCache Is Nothing Then
					vocabCache = New AbstractCache()
				End If
				Dim words As IList(Of String) = fastTextImpl.getWords()
				For i As Integer = 0 To words.Count - 1
					vocabCache.addWordToIndex(i, words(i))
					Dim word As New VocabWord()
					word.Word = words(i)
					vocabCache.addToken(word)
				Next i
			End If
			Return vocabCache
		End Function

		Public Overridable Function vocabSize() As Long
			Dim result As Long = 0
			If modelVectorsLoaded Then
				result = word2Vec.vocabSize()
			Else
				If Not modelLoaded Then
					Throw New System.InvalidOperationException("Load model before calling vocab()")
				End If
				result = fastTextImpl.getNWords()
			End If
			Return result
		End Function

		Public Overridable Property UNK As String Implements WordVectors.getUNK
			Get
				Throw New NotImplementedException("FastText.getUNK")
			End Get
			Set(ByVal input As String)
				Throw New NotImplementedException("FastText.setUNK")
			End Set
		End Property


		Public Overridable Function getWordVector(ByVal word As String) As Double() Implements WordVectors.getWordVector
			If modelVectorsLoaded Then
				Return word2Vec.getWordVector(word)
			Else
				Dim vectors As IList(Of Single) = fastTextImpl.getVector(word)
				Dim retVal(vectors.Count - 1) As Double
				For i As Integer = 0 To vectors.Count - 1
					retVal(i) = vectors(i)
				Next i
				Return retVal
			End If
		End Function

		Public Overridable Function getWordVectorMatrixNormalized(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrixNormalized
			If modelVectorsLoaded Then
				Return word2Vec.getWordVectorMatrixNormalized(word)
			Else
				Dim r As INDArray = getWordVectorMatrix(word)
				Return r.divi(Nd4j.BlasWrapper.nrm2(r))
			End If
		End Function

		Public Overridable Function getWordVectorMatrix(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrix
			If modelVectorsLoaded Then
				Return word2Vec.getWordVectorMatrix(word)
			Else
				Dim values() As Double = getWordVector(word)
				Return Nd4j.createFromArray(values)
			End If
		End Function

		Public Overridable Function getWordVectors(ByVal labels As ICollection(Of String)) As INDArray
			If modelVectorsLoaded Then
				Return word2Vec.getWordVectors(labels)
			End If
			Return Nothing
		End Function

		Public Overridable Function getWordVectorsMean(ByVal labels As ICollection(Of String)) As INDArray
			If modelVectorsLoaded Then
				Return word2Vec.getWordVectorsMean(labels)
			End If
			Return Nothing
		End Function

		Private words As IList(Of String) = New List(Of String)()

		Public Overridable Function hasWord(ByVal word As String) As Boolean Implements WordVectors.hasWord
			If modelVectorsLoaded Then
				Return word2Vec.outOfVocabularySupported()
			End If
			If words.Count = 0 Then
				words = fastTextImpl.getWords()
			End If
			Return words.Contains(word)
		End Function

		Public Overridable Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.wordsNearest(words, top)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function

		Public Overridable Function wordsNearestSum(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.wordsNearestSum(words, top)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function

		Public Overridable Function wordsNearestSum(ByVal word As String, ByVal n As Integer) As ICollection(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.wordsNearestSum(word, n)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function


		Public Overridable Function wordsNearestSum(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.wordsNearestSum(positive, negative, top)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function

		Public Overridable Function accuracy(ByVal questions As IList(Of String)) As IDictionary(Of String, Double)
			If modelVectorsLoaded Then
				Return word2Vec.accuracy(questions)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function

		Public Overridable Function indexOf(ByVal word As String) As Integer Implements WordVectors.indexOf
			If modelVectorsLoaded Then
				Return word2Vec.indexOf(word)
			End If
			Return vocab().indexOf(word)
		End Function


		Public Overridable Function similarWordsInVocabTo(ByVal word As String, ByVal accuracy As Double) As IList(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.similarWordsInVocabTo(word, accuracy)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function

		Public Overridable Function wordsNearest(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.wordsNearest(positive, negative, top)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function


		Public Overridable Function wordsNearest(ByVal word As String, ByVal n As Integer) As ICollection(Of String)
			If modelVectorsLoaded Then
				Return word2Vec.wordsNearest(word,n)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function


		Public Overridable Function similarity(ByVal word As String, ByVal word2 As String) As Double Implements WordVectors.similarity
			If modelVectorsLoaded Then
				Return word2Vec.similarity(word, word2)
			End If
			Throw New System.InvalidOperationException(METHOD_NOT_AVAILABLE)
		End Function

		Public Overridable Function lookupTable() As WeightLookupTable Implements WordVectors.lookupTable
			If modelVectorsLoaded Then
				Return word2Vec.lookupTable()
			End If
			Return Nothing
		End Function

		Public Overridable WriteOnly Property ModelUtils Implements WordVectors.setModelUtils As ModelUtils
			Set(ByVal utils As ModelUtils)
			End Set
		End Property

		Public Overridable Sub loadWeightsInto(ByVal array As INDArray)
		End Sub
		Public Overridable Function vectorSize() As Integer
			Return -1
		End Function
		Public Overridable Function jsonSerializable() As Boolean
			Return False
		End Function

		Public Overridable ReadOnly Property LearningRate As Double
			Get
				Return fastTextImpl.getLr()
			End Get
		End Property

		Public Overridable ReadOnly Property Dimension As Integer
			Get
				Return fastTextImpl.getDim()
			End Get
		End Property

		Public Overridable ReadOnly Property ContextWindowSize As Integer
			Get
				Return fastTextImpl.getContextWindowSize()
			End Get
		End Property

		Public Overridable ReadOnly Property Epoch As Integer
			Get
				Return fastTextImpl.getEpoch()
			End Get
		End Property

		Public Overridable ReadOnly Property NegativesNumber As Integer
			Get
				Return fastTextImpl.getNSampledNegatives()
			End Get
		End Property

		Public Overridable ReadOnly Property WordNgrams As Integer
			Get
				Return fastTextImpl.getWordNgrams()
			End Get
		End Property

		Public Overridable ReadOnly Property LossName As String
			Get
				Return fastTextImpl.getLossName()
			End Get
		End Property

		Public Overridable ReadOnly Property ModelName As String
			Get
				Return fastTextImpl.getModelName()
			End Get
		End Property

		Public Overridable ReadOnly Property NumberOfBuckets As Integer
			Get
				Return fastTextImpl.getBucket()
			End Get
		End Property

		Public Overridable ReadOnly Property LabelPrefix As String
			Get
				Return fastTextImpl.getLabelPrefix()
			End Get
		End Property

		Public Overridable Function outOfVocabularySupported() As Boolean Implements WordVectors.outOfVocabularySupported
			Return True
		End Function

	End Class

End Namespace