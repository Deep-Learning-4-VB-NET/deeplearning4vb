Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Base64 = org.apache.commons.codec.binary.Base64
Imports GzipUtils = org.apache.commons.compress.compressors.gzip.GzipUtils
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports CloseShieldOutputStream = org.apache.commons.io.output.CloseShieldOutputStream
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports DL4JFileUtils = org.deeplearning4j.common.util.DL4JFileUtils
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.embeddings.wordvectors
Imports FastText = org.deeplearning4j.models.fasttext.FastText
Imports ParagraphVectors = org.deeplearning4j.models.paragraphvectors.ParagraphVectors
Imports org.deeplearning4j.models.sequencevectors
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports VocabWordFactory = org.deeplearning4j.models.sequencevectors.serialization.VocabWordFactory
Imports StaticWord2Vec = org.deeplearning4j.models.word2vec.StaticWord2Vec
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore
Imports VocabularyHolder = org.deeplearning4j.models.word2vec.wordstore.VocabularyHolder
Imports VocabularyWord = org.deeplearning4j.models.word2vec.wordstore.VocabularyWord
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports InMemoryLookupCache = org.deeplearning4j.models.word2vec.wordstore.inmemory.InMemoryLookupCache
Imports LabelsSource = org.deeplearning4j.text.documentiterator.LabelsSource
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports org.nd4j.common.primitives
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger
Imports NoOp = org.nd4j.compression.impl.NoOp
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports org.nd4j.storage

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

Namespace org.deeplearning4j.models.embeddings.loader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class WordVectorSerializer
	Public Class WordVectorSerializer
		Private Const MAX_SIZE As Integer = 50
		Private Const WHITESPACE_REPLACEMENT As String = "_Az92_"

		Private Sub New()
		End Sub

		''' <param name="modelFile">
		''' @return </param>
		''' <exception cref="FileNotFoundException"> </exception>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="NumberFormatException"> </exception>
	'    private static Word2Vec readTextModel(File modelFile) throws IOException, NumberFormatException {
	'        InMemoryLookupTable lookupTable;
	'        VocabCache cache;
	'        INDArray syn0;
	'        Word2Vec ret = new Word2Vec();
	'        try (BufferedReader reader =
	'                     new BufferedReader(new InputStreamReader(GzipUtils.isCompressedFilename(modelFile.getName())
	'                             ? new GZIPInputStream(new FileInputStream(modelFile))
	'                             : new FileInputStream(modelFile), "UTF-8"))) {
	'            String line = reader.readLine();
	'            String[] initial = line.split(" ");
	'            int words = Integer.parseInt(initial[0]);
	'            int layerSize = Integer.parseInt(initial[1]);
	'            syn0 = Nd4j.create(words, layerSize);
	'
	'            cache = new InMemoryLookupCache(false);
	'
	'            int currLine = 0;
	'            while ((line = reader.readLine()) != null) {
	'                String[] split = line.split(" ");
	'                Preconditions.checkState(split.length == layerSize + 1, "Expected %s values, got %s", layerSize + 1, split.length);
	'                String word = split[0].replaceAll(WHITESPACE_REPLACEMENT, " ");
	'
	'                float[] vector = new float[split.length - 1];
	'                for (int i = 1; i < split.length; i++) {
	'                    vector[i - 1] = Float.parseFloat(split[i]);
	'                }
	'
	'                syn0.putRow(currLine, Nd4j.create(vector));
	'
	'                cache.addWordToIndex(cache.numWords(), word);
	'                cache.addToken(new VocabWord(1, word));
	'                cache.putVocabWord(word);
	'
	'                currLine++;
	'            }
	'
	'            lookupTable = (InMemoryLookupTable) new InMemoryLookupTable.Builder().cache(cache).vectorLength(layerSize)
	'                    .build();
	'            lookupTable.setSyn0(syn0);
	'
	'            ret.setVocab(cache);
	'            ret.setLookupTable(lookupTable);
	'        }
	'        return ret;
	'    }

		''' <summary>
		''' Read a binary word2vec from input stream.
		''' </summary>
		''' <param name="inputStream">  input stream to read </param>
		''' <param name="linebreaks">  if true, the reader expects each word/vector to be in a separate line, terminated
		'''      by a line break </param>
		''' <param name="normalize">
		''' </param>
		''' <returns> a <seealso cref="Word2Vec model"/> </returns>
		''' <exception cref="NumberFormatException"> </exception>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="FileNotFoundException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readBinaryModel(java.io.InputStream inputStream, boolean linebreaks, boolean normalize) throws NumberFormatException, java.io.IOException
		Public Shared Function readBinaryModel(ByVal inputStream As Stream, ByVal linebreaks As Boolean, ByVal normalize As Boolean) As Word2Vec
			Dim lookupTable As InMemoryLookupTable(Of VocabWord)
			Dim cache As VocabCache(Of VocabWord)
			Dim syn0 As INDArray
			Dim words, size As Integer

			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive

			If originalPeriodic Then
				Nd4j.MemoryManager.togglePeriodicGc(False)
			End If

			Nd4j.MemoryManager.OccasionalGcFrequency = 50000

			Try
					Using dis As New DataInputStream(inputStream)
					words = Integer.Parse(ReadHelper.readString(dis))
					size = Integer.Parse(ReadHelper.readString(dis))
					syn0 = Nd4j.create(words, size)
					cache = New AbstractCache(Of VocabWord)()
        
					printOutProjectedMemoryUse(words, size, 1)
        
					lookupTable = (New InMemoryLookupTable.Builder(Of VocabWord)()).cache(cache).useHierarchicSoftmax(False).vectorLength(size).build()
        
					Dim word As String
					Dim vector(size - 1) As Single
					For i As Integer = 0 To words - 1
						word = ReadHelper.readString(dis)
						log.trace("Loading {} with word {}", word, i)
        
						For j As Integer = 0 To size - 1
							vector(j) = ReadHelper.readFloat(dis)
						Next j
        
						If cache.containsWord(word) Then
							Throw New ND4JIllegalStateException("Tried to add existing word. Probably time to switch linebreaks mode?")
						End If
        
						syn0.putRow(i,If(normalize, Transforms.unitVec(Nd4j.create(vector)), Nd4j.create(vector)))
        
						Dim vw As New VocabWord(1.0, word)
						vw.Index = cache.numWords()
        
						cache.addToken(vw)
						cache.addWordToIndex(vw.Index, vw.Label)
        
						cache.putVocabWord(word)
        
						If linebreaks Then
							dis.readByte() ' line break
						End If
        
						Nd4j.MemoryManager.invokeGcOccasionally()
					Next i
					End Using
			Finally
				If originalPeriodic Then
					Nd4j.MemoryManager.togglePeriodicGc(True)
				End If

				Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq
			End Try

			lookupTable.setSyn0(syn0)

			Dim ret As Word2Vec = (New Word2Vec.Builder()).useHierarchicSoftmax(False).resetModel(False).layerSize(syn0.columns()).allowParallelTokenization(True).elementsLearningAlgorithm(New SkipGram(Of VocabWord)()).learningRate(0.025).windowSize(5).workers(1).build()

			ret.Vocab = cache
			ret.LookupTable = lookupTable

			Return ret
		End Function

		''' <summary>
		''' This method writes word vectors to the given path.
		''' Please note: this method doesn't load whole vocab/lookupTable into memory, so it's able to process large vocabularies served over network.
		''' </summary>
		''' <param name="lookupTable"> </param>
		''' <param name="path"> </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeWordVectors(org.deeplearning4j.models.embeddings.WeightLookupTable<T> lookupTable, String path) throws java.io.IOException
		Public Shared Sub writeWordVectors(Of T As SequenceElement)(ByVal lookupTable As WeightLookupTable(Of T), ByVal path As String)
			Try
					Using bos As New BufferedOutputStream(New FileStream(path, FileMode.Create, FileAccess.Write))
					writeWordVectors(lookupTable, bos)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method writes word vectors to the given file.
		''' Please note: this method doesn't load whole vocab/lookupTable into memory, so it's able to process large vocabularies served over network.
		''' </summary>
		''' <param name="lookupTable"> </param>
		''' <param name="file"> </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeWordVectors(org.deeplearning4j.models.embeddings.WeightLookupTable<T> lookupTable, java.io.File file) throws java.io.IOException
		Public Shared Sub writeWordVectors(Of T As SequenceElement)(ByVal lookupTable As WeightLookupTable(Of T), ByVal file As File)
			Try
					Using bos As New BufferedOutputStream(New FileStream(file, FileMode.Create, FileAccess.Write))
					writeWordVectors(lookupTable, bos)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method writes word vectors to the given OutputStream.
		''' Please note: this method doesn't load whole vocab/lookupTable into memory, so it's able to process large vocabularies served over network.
		''' </summary>
		''' <param name="lookupTable"> </param>
		''' <param name="stream"> </param>
		''' @param <T> </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeWordVectors(org.deeplearning4j.models.embeddings.WeightLookupTable<T> lookupTable, java.io.OutputStream stream) throws java.io.IOException
		Public Shared Sub writeWordVectors(Of T As SequenceElement)(ByVal lookupTable As WeightLookupTable(Of T), ByVal stream As Stream)
			Dim vocabCache As val = lookupTable.getVocabCache()

			Using writer As New java.io.PrintWriter(New StreamWriter(stream, Encoding.UTF8))
				' saving header as "NUM_WORDS VECTOR_SIZE NUM_DOCS"
				Dim str As val = vocabCache.numWords() & " " & lookupTable.layerSize() & " " & vocabCache.totalNumberOfDocs()
				log.debug("Saving header: {}", str)
				writer.println(str)

				' saving vocab content
				Dim num As val = vocabCache.numWords()
				For x As Integer = 0 To num - 1
					Dim element As T = vocabCache.elementAtIndex(x)

					Dim builder As val = New StringBuilder()

					Dim l As val = element.getLabel()
					builder.append(ReadHelper.encodeB64(l)).append(" ")
					Dim vec As val = lookupTable.vector(element.getLabel())
					For i As Integer = 0 To vec.length() - 1
						builder.append(vec.getDouble(i))
						If i < vec.length() - 1 Then
							builder.append(" ")
						End If
					Next i
					writer.println(builder.ToString())
				Next x
			End Using
		End Sub


		''' <summary>
		''' This method saves paragraph vectors to the given file.
		''' </summary>
		''' @deprecated Use <seealso cref="writeParagraphVectors(ParagraphVectors, File)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeParagraphVectors(ParagraphVectors, File)""/>") public static void writeWordVectors(@NonNull ParagraphVectors vectors, @NonNull File path)
		<Obsolete("Use <seealso cref=""writeParagraphVectors(ParagraphVectors, File)""/>")>
		Public Shared Sub writeWordVectors(ByVal vectors As ParagraphVectors, ByVal path As File)
			Try
					Using fos As New FileStream(path, FileMode.Create, FileAccess.Write)
					writeWordVectors(vectors, fos)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub


		''' <summary>
		''' This method saves paragraph vectors to the given path.
		''' </summary>
		''' @deprecated Use <seealso cref="writeParagraphVectors(ParagraphVectors, String)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeParagraphVectors(ParagraphVectors, String)""/>") public static void writeWordVectors(@NonNull ParagraphVectors vectors, @NonNull String path)
		<Obsolete("Use <seealso cref=""writeParagraphVectors(ParagraphVectors, String)""/>")>
		Public Shared Sub writeWordVectors(ByVal vectors As ParagraphVectors, ByVal path As String)
			Try
					Using fos As New FileStream(path, FileMode.Create, FileAccess.Write)
					writeWordVectors(vectors, fos)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method saves ParagraphVectors model into compressed zip file
		''' </summary>
		''' <param name="file"> </param>
		Public Shared Sub writeParagraphVectors(ByVal vectors As ParagraphVectors, ByVal file As File)
			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using fos As System.IO.FileStream_Output = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write), stream As java.io.BufferedOutputStream = new java.io.BufferedOutputStream(fos)
					New FileStream(file, FileMode.Create, FileAccess.Write), stream As New BufferedOutputStream(fos)
						Using fos As New FileStream(file, FileMode.Create, FileAccess.Write), stream As BufferedOutputStream
					writeParagraphVectors(vectors, stream)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method saves ParagraphVectors model into compressed zip file located at path
		''' </summary>
		''' <param name="path"> </param>
		Public Shared Sub writeParagraphVectors(ByVal vectors As ParagraphVectors, ByVal path As String)
			writeParagraphVectors(vectors, New File(path))
		End Sub

		''' <summary>
		''' This method saves Word2Vec model into compressed zip file
		''' PLEASE NOTE: This method saves FULL model, including syn0 AND syn1
		''' </summary>
		Public Shared Sub writeWord2VecModel(ByVal vectors As Word2Vec, ByVal file As File)
			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using fos As System.IO.FileStream_Output = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write), stream As java.io.BufferedOutputStream = new java.io.BufferedOutputStream(fos)
					New FileStream(file, FileMode.Create, FileAccess.Write), stream As New BufferedOutputStream(fos)
						Using fos As New FileStream(file, FileMode.Create, FileAccess.Write), stream As BufferedOutputStream
					writeWord2VecModel(vectors, stream)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method saves Word2Vec model into compressed zip file
		''' PLEASE NOTE: This method saves FULL model, including syn0 AND syn1
		''' </summary>
		Public Shared Sub writeWord2VecModel(ByVal vectors As Word2Vec, ByVal path As String)
			writeWord2VecModel(vectors, New File(path))
		End Sub

		''' <summary>
		''' This method saves Word2Vec model into compressed zip file and sends it to output stream
		''' PLEASE NOTE: This method saves FULL model, including syn0 AND syn1
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeWord2VecModel(org.deeplearning4j.models.word2vec.Word2Vec vectors, java.io.OutputStream stream) throws java.io.IOException
		Public Shared Sub writeWord2VecModel(ByVal vectors As Word2Vec, ByVal stream As Stream)
			Dim zipfile As New ZipOutputStream(New BufferedOutputStream(New CloseShieldOutputStream(stream)))

			Dim syn0 As New ZipEntry("syn0.txt")
			zipfile.putNextEntry(syn0)

			' writing out syn0
			Dim tempFileSyn0 As File = DL4JFileUtils.createTempFile("word2vec", "0")
			Dim tempFileSyn1 As File = DL4JFileUtils.createTempFile("word2vec", "1")
			Dim tempFileSyn1Neg As File = DL4JFileUtils.createTempFile("word2vec", "n")
			Dim tempFileCodes As File = DL4JFileUtils.createTempFile("word2vec", "h")
			Dim tempFileHuffman As File = DL4JFileUtils.createTempFile("word2vec", "h")
			Dim tempFileFreqs As File = DL4JFileUtils.createTempFile("word2vec", "f")
			tempFileSyn0.deleteOnExit()
			tempFileSyn1.deleteOnExit()
			tempFileSyn1Neg.deleteOnExit()
			tempFileFreqs.deleteOnExit()
			tempFileCodes.deleteOnExit()
			tempFileHuffman.deleteOnExit()

			Try
				writeWordVectors(vectors.lookupTable(), tempFileSyn0)

				FileUtils.copyFile(tempFileSyn0, zipfile)

				' writing out syn1
				Dim syn1 As INDArray = CType(vectors.getLookupTable(), InMemoryLookupTable(Of VocabWord)).getSyn1()

				If syn1 IsNot Nothing Then
					Using writer As New java.io.PrintWriter(New StreamWriter(tempFileSyn1))
						Dim x As Integer = 0
						Do While x < syn1.rows()
							Dim row As INDArray = syn1.getRow(x)
							Dim builder As New StringBuilder()
							For i As Integer = 0 To row.length() - 1
								builder.Append(row.getDouble(i)).Append(" ")
							Next i
							writer.println(builder.ToString().Trim())
							x += 1
						Loop
					End Using
				End If

				Dim zSyn1 As New ZipEntry("syn1.txt")
				zipfile.putNextEntry(zSyn1)

				FileUtils.copyFile(tempFileSyn1, zipfile)

				' writing out syn1
				Dim syn1Neg As INDArray = CType(vectors.getLookupTable(), InMemoryLookupTable(Of VocabWord)).Syn1Neg

				If syn1Neg IsNot Nothing Then
					Using writer As New java.io.PrintWriter(New StreamWriter(tempFileSyn1Neg))
						Dim x As Integer = 0
						Do While x < syn1Neg.rows()
							Dim row As INDArray = syn1Neg.getRow(x)
							Dim builder As New StringBuilder()
							For i As Integer = 0 To row.length() - 1
								builder.Append(row.getDouble(i)).Append(" ")
							Next i
							writer.println(builder.ToString().Trim())
							x += 1
						Loop
					End Using
				End If

				Dim zSyn1Neg As New ZipEntry("syn1Neg.txt")
				zipfile.putNextEntry(zSyn1Neg)

				FileUtils.copyFile(tempFileSyn1Neg, zipfile)


				Dim hC As New ZipEntry("codes.txt")
				zipfile.putNextEntry(hC)

				' writing out huffman tree
				Using writer As New java.io.PrintWriter(New StreamWriter(tempFileCodes))
					Dim i As Integer = 0
					Do While i < vectors.getVocab().numWords()
						Dim word As VocabWord = vectors.getVocab().elementAtIndex(i)
						Dim builder As StringBuilder = (New StringBuilder(ReadHelper.encodeB64(word.Label))).Append(" ")
						For Each code As Integer In word.getCodes()
							builder.Append(code).Append(" ")
						Next code

						writer.println(builder.ToString().Trim())
						i += 1
					Loop
				End Using

				FileUtils.copyFile(tempFileCodes, zipfile)

				Dim hP As New ZipEntry("huffman.txt")
				zipfile.putNextEntry(hP)

				' writing out huffman tree
				Using writer As New java.io.PrintWriter(New StreamWriter(tempFileHuffman))
					Dim i As Integer = 0
					Do While i < vectors.getVocab().numWords()
						Dim word As VocabWord = vectors.getVocab().elementAtIndex(i)
						Dim builder As StringBuilder = (New StringBuilder(ReadHelper.encodeB64(word.Label))).Append(" ")
						For Each point As Integer In word.getPoints()
							builder.Append(point).Append(" ")
						Next point

						writer.println(builder.ToString().Trim())
						i += 1
					Loop
				End Using

				FileUtils.copyFile(tempFileHuffman, zipfile)

				Dim hF As New ZipEntry("frequencies.txt")
				zipfile.putNextEntry(hF)

				' writing out word frequencies
				Using writer As New java.io.PrintWriter(New StreamWriter(tempFileFreqs))
					Dim i As Integer = 0
					Do While i < vectors.getVocab().numWords()
						Dim word As VocabWord = vectors.getVocab().elementAtIndex(i)
						Dim builder As StringBuilder = (New StringBuilder(ReadHelper.encodeB64(word.Label))).Append(" ").Append(word.getElementFrequency()).Append(" ").Append(vectors.getVocab().docAppearedIn(word.Label))

						writer.println(builder.ToString().Trim())
						i += 1
					Loop
				End Using

				FileUtils.copyFile(tempFileFreqs, zipfile)

				Dim config As New ZipEntry("config.json")
				zipfile.putNextEntry(config)
				'log.info("Current config: {}", vectors.getConfiguration().toJson());
				Using bais As New MemoryStream(vectors.getConfiguration().toJson().getBytes(java.nio.charset.StandardCharsets.UTF_8))
					IOUtils.copy(bais, zipfile)
				End Using

				zipfile.flush()
				zipfile.close()
			Finally
				For Each f As File In New File(){tempFileSyn0, tempFileSyn1, tempFileSyn1Neg, tempFileCodes, tempFileHuffman, tempFileFreqs}
					Try
						f.delete()
					Catch e As Exception
						'Ignore, is temp file
					End Try
				Next f
			End Try
		End Sub

		''' <summary>
		''' This method saves ParagraphVectors model into compressed zip file and sends it to output stream
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeParagraphVectors(org.deeplearning4j.models.paragraphvectors.ParagraphVectors vectors, java.io.OutputStream stream) throws java.io.IOException
		Public Shared Sub writeParagraphVectors(ByVal vectors As ParagraphVectors, ByVal stream As Stream)
			Dim zipfile As New ZipOutputStream(New BufferedOutputStream(New CloseShieldOutputStream(stream)))

			Dim syn0 As New ZipEntry("syn0.txt")
			zipfile.putNextEntry(syn0)

			' writing out syn0
			Dim tempFileSyn0 As File = DL4JFileUtils.createTempFile("paravec", "0")
			Dim tempFileSyn1 As File = DL4JFileUtils.createTempFile("paravec", "1")
			Dim tempFileCodes As File = DL4JFileUtils.createTempFile("paravec", "h")
			Dim tempFileHuffman As File = DL4JFileUtils.createTempFile("paravec", "h")
			Dim tempFileFreqs As File = DL4JFileUtils.createTempFile("paravec", "h")
			tempFileSyn0.deleteOnExit()
			tempFileSyn1.deleteOnExit()
			tempFileCodes.deleteOnExit()
			tempFileHuffman.deleteOnExit()
			tempFileFreqs.deleteOnExit()

			Try

				writeWordVectors(vectors.lookupTable(), tempFileSyn0)

				FileUtils.copyFile(tempFileSyn0, zipfile)

				' writing out syn1
				Dim syn1 As INDArray = CType(vectors.getLookupTable(), InMemoryLookupTable(Of VocabWord)).getSyn1()

				If syn1 IsNot Nothing Then
					Using writer As New java.io.PrintWriter(New StreamWriter(tempFileSyn1))
						Dim x As Integer = 0
						Do While x < syn1.rows()
							Dim row As INDArray = syn1.getRow(x)
							Dim builder As New StringBuilder()
							For i As Integer = 0 To row.length() - 1
								builder.Append(row.getDouble(i)).Append(" ")
							Next i
							writer.println(builder.ToString().Trim())
							x += 1
						Loop
					End Using
				End If

				Dim zSyn1 As New ZipEntry("syn1.txt")
				zipfile.putNextEntry(zSyn1)

				FileUtils.copyFile(tempFileSyn1, zipfile)

				Dim hC As New ZipEntry("codes.txt")
				zipfile.putNextEntry(hC)

				' writing out huffman tree
				Using writer As New java.io.PrintWriter(New StreamWriter(tempFileCodes))
					Dim i As Integer = 0
					Do While i < vectors.getVocab().numWords()
						Dim word As VocabWord = vectors.getVocab().elementAtIndex(i)
						Dim builder As StringBuilder = (New StringBuilder(ReadHelper.encodeB64(word.Label))).Append(" ")
						For Each code As Integer In word.getCodes()
							builder.Append(code).Append(" ")
						Next code

						writer.println(builder.ToString().Trim())
						i += 1
					Loop
				End Using

				FileUtils.copyFile(tempFileCodes, zipfile)

				Dim hP As New ZipEntry("huffman.txt")
				zipfile.putNextEntry(hP)

				' writing out huffman tree
				Using writer As New java.io.PrintWriter(New StreamWriter(tempFileHuffman))
					Dim i As Integer = 0
					Do While i < vectors.getVocab().numWords()
						Dim word As VocabWord = vectors.getVocab().elementAtIndex(i)
						Dim builder As StringBuilder = (New StringBuilder(ReadHelper.encodeB64(word.Label))).Append(" ")
						For Each point As Integer In word.getPoints()
							builder.Append(point).Append(" ")
						Next point

						writer.println(builder.ToString().Trim())
						i += 1
					Loop
				End Using

				FileUtils.copyFile(tempFileHuffman, zipfile)

				Dim config As New ZipEntry("config.json")
				zipfile.putNextEntry(config)
				IOUtils.write(vectors.getConfiguration().toJson(), zipfile, StandardCharsets.UTF_8)


				Dim labels As New ZipEntry("labels.txt")
				zipfile.putNextEntry(labels)
				Dim builder As New StringBuilder()
				For Each word As VocabWord In vectors.getVocab().tokens()
					If word.Label Then
						builder.Append(ReadHelper.encodeB64(word.Label)).Append(vbLf)
					End If
				Next word
				IOUtils.write(builder.ToString().Trim(), zipfile, StandardCharsets.UTF_8)

				Dim hF As New ZipEntry("frequencies.txt")
				zipfile.putNextEntry(hF)

				' writing out word frequencies
				Using writer As New java.io.PrintWriter(New StreamWriter(tempFileFreqs))
					Dim i As Integer = 0
					Do While i < vectors.getVocab().numWords()
						Dim word As VocabWord = vectors.getVocab().elementAtIndex(i)
						builder = (New StringBuilder(ReadHelper.encodeB64(word.Label))).Append(" ").Append(word.getElementFrequency()).Append(" ").Append(vectors.getVocab().docAppearedIn(word.Label))

						writer.println(builder.ToString().Trim())
						i += 1
					Loop
				End Using

				FileUtils.copyFile(tempFileFreqs, zipfile)

				zipfile.flush()
				zipfile.close()
			Finally
				For Each f As File In New File(){tempFileSyn0, tempFileSyn1, tempFileCodes, tempFileHuffman, tempFileFreqs}
					Try
						f.delete()
					Catch e As Exception
						'Ignore, is temp file
					End Try
				Next f
			End Try
		End Sub


		''' <summary>
		''' This method restores ParagraphVectors model previously saved with writeParagraphVectors()
		''' 
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.models.paragraphvectors.ParagraphVectors readParagraphVectors(String path) throws java.io.IOException
		Public Shared Function readParagraphVectors(ByVal path As String) As ParagraphVectors
			Return readParagraphVectors(New File(path))
		End Function


		''' <summary>
		''' This method restores ParagraphVectors model previously saved with writeParagraphVectors()
		''' 
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.models.paragraphvectors.ParagraphVectors readParagraphVectors(java.io.File file) throws java.io.IOException
		Public Shared Function readParagraphVectors(ByVal file As File) As ParagraphVectors
			Dim w2v As Word2Vec = readWord2Vec(file)

			' and "convert" it to ParaVec model + optionally trying to restore labels information
			Dim vectors As ParagraphVectors = (New ParagraphVectors.Builder(w2v.getConfiguration())).vocabCache(w2v.getVocab()).lookupTable(w2v.getLookupTable()).resetModel(False).build()

			Using zipFile As New java.util.zip.ZipFile(file)
				' now we try to restore labels information
				Dim labels As ZipEntry = zipFile.getEntry("labels.txt")
				If labels IsNot Nothing Then
					Dim stream As Stream = zipFile.getInputStream(labels)
					Using reader As New StreamReader(stream, Encoding.UTF8)
						Dim line As String
						line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
						Do While line IsNot Nothing
							Dim word As VocabWord = vectors.getVocab().tokenFor(ReadHelper.decodeB64(line.Trim()))
							If word IsNot Nothing Then
								word.markAsLabel(True)
							End If
								line = reader.ReadLine()
						Loop
					End Using
				End If
			End Using

			vectors.extractLabels()

			Return vectors
		End Function

		''' <summary>
		''' This method restores Word2Vec model previously saved with writeWord2VecModel
		''' <para>
		''' PLEASE NOTE: This method loads FULL model, so don't use it if you're only going to use weights.
		''' 
		''' </para>
		''' </summary>
		''' <param name="file">
		''' @return </param>
		''' @deprecated Use <seealso cref="readWord2Vec(File, Boolean)"/> 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""readWord2Vec(File, Boolean)""/>") public static org.deeplearning4j.models.word2vec.Word2Vec readWord2Vec(java.io.File file) throws java.io.IOException
		<Obsolete("Use <seealso cref=""readWord2Vec(File, Boolean)""/>")>
		Public Shared Function readWord2Vec(ByVal file As File) As Word2Vec
			Dim tmpFileSyn0 As File = DL4JFileUtils.createTempFile("word2vec", "0")
			Dim tmpFileSyn1 As File = DL4JFileUtils.createTempFile("word2vec", "1")
			Dim tmpFileC As File = DL4JFileUtils.createTempFile("word2vec", "c")
			Dim tmpFileH As File = DL4JFileUtils.createTempFile("word2vec", "h")
			Dim tmpFileF As File = DL4JFileUtils.createTempFile("word2vec", "f")

			tmpFileSyn0.deleteOnExit()
			tmpFileSyn1.deleteOnExit()
			tmpFileH.deleteOnExit()
			tmpFileC.deleteOnExit()
			tmpFileF.deleteOnExit()

			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive

			If originalPeriodic Then
				Nd4j.MemoryManager.togglePeriodicGc(False)
			End If

			Nd4j.MemoryManager.OccasionalGcFrequency = 50000

			Try


				Dim zipFile As New ZipFile(file)
				Dim syn0 As ZipEntry = zipFile.getEntry("syn0.txt")
				Dim stream As Stream = zipFile.getInputStream(syn0)

				FileUtils.copyInputStreamToFile(stream, tmpFileSyn0)

				Dim syn1 As ZipEntry = zipFile.getEntry("syn1.txt")
				stream = zipFile.getInputStream(syn1)

				FileUtils.copyInputStreamToFile(stream, tmpFileSyn1)

				Dim codes As ZipEntry = zipFile.getEntry("codes.txt")
				stream = zipFile.getInputStream(codes)

				FileUtils.copyInputStreamToFile(stream, tmpFileC)

				Dim huffman As ZipEntry = zipFile.getEntry("huffman.txt")
				stream = zipFile.getInputStream(huffman)

				FileUtils.copyInputStreamToFile(stream, tmpFileH)

				Dim config As ZipEntry = zipFile.getEntry("config.json")
				stream = zipFile.getInputStream(config)
				Dim builder As New StringBuilder()
				Using reader As New StreamReader(stream)
					Dim line As String
					line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
					Do While line IsNot Nothing
						builder.Append(line)
							line = reader.ReadLine()
					Loop
				End Using

				Dim configuration As VectorsConfiguration = VectorsConfiguration.fromJson(builder.ToString().Trim())

				' we read first 4 files as w2v model
				Dim w2v As Word2Vec = readWord2VecFromText(tmpFileSyn0, tmpFileSyn1, tmpFileC, tmpFileH, configuration)

				' we read frequencies from frequencies.txt, however it's possible that we might not have this file
				Dim frequencies As ZipEntry = zipFile.getEntry("frequencies.txt")
				If frequencies IsNot Nothing Then
					stream = zipFile.getInputStream(frequencies)
					Using reader As New StreamReader(stream)
						Dim line As String
						line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
						Do While line IsNot Nothing
							Dim split() As String = line.Split(" ", True)
							Dim word As VocabWord = w2v.getVocab().tokenFor(ReadHelper.decodeB64(split(0)))
							word.setElementFrequency(CLng(Math.Truncate(Double.Parse(split(1)))))
							word.SequencesCount = CLng(Math.Truncate(Double.Parse(split(2))))
								line = reader.ReadLine()
						Loop
					End Using
				End If


				Dim zsyn1Neg As ZipEntry = zipFile.getEntry("syn1Neg.txt")
				If zsyn1Neg IsNot Nothing Then
					stream = zipFile.getInputStream(zsyn1Neg)

					Using isr As New StreamReader(stream), reader As New StreamReader(isr)
						Dim line As String = Nothing
						Dim rows As IList(Of INDArray) = New List(Of INDArray)()
						line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
						Do While line IsNot Nothing
							Dim split() As String = line.Split(" ", True)
							Dim array(split.Length - 1) As Double
							For i As Integer = 0 To split.Length - 1
								array(i) = Double.Parse(split(i))
							Next i
							rows.Add(Nd4j.create(array, New Long(){array.Length}, CType(w2v.getLookupTable(), InMemoryLookupTable).getSyn0().dataType()))
								line = reader.ReadLine()
						Loop

						' it's possible to have full model without syn1Neg
						If rows.Count > 0 Then
							Dim syn1Neg As INDArray = Nd4j.vstack(rows)
							CType(w2v.getLookupTable(), InMemoryLookupTable).setSyn1Neg(syn1Neg)
						End If
					End Using
				End If

				Return w2v
			Finally
				If originalPeriodic Then
					Nd4j.MemoryManager.togglePeriodicGc(True)
				End If

				Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq

				For Each f As File In New File(){tmpFileSyn0, tmpFileSyn1, tmpFileC, tmpFileH, tmpFileF}
					Try
						f.delete()
					Catch e As Exception
						'Ignore, is temp file
					End Try
				Next f
			End Try
		End Function

		''' <summary>
		''' This method restores ParagraphVectors model previously saved with writeParagraphVectors()
		''' 
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.models.paragraphvectors.ParagraphVectors readParagraphVectors(java.io.InputStream stream) throws java.io.IOException
		Public Shared Function readParagraphVectors(ByVal stream As Stream) As ParagraphVectors
			Dim tmpFile As File = DL4JFileUtils.createTempFile("restore", "paravec")
			Try
				FileUtils.copyInputStreamToFile(stream, tmpFile)
				Return readParagraphVectors(tmpFile)
			Finally
				tmpFile.delete()
			End Try
		End Function

		''' <summary>
		''' This method allows you to read ParagraphVectors from externally originated vectors and syn1.
		''' So, technically this method is compatible with any other w2v implementation
		''' </summary>
		''' <param name="vectors">  text file with words and their weights, aka Syn0 </param>
		''' <param name="hs">       text file HS layers, aka Syn1 </param>
		''' <param name="h_codes">  text file with Huffman tree codes </param>
		''' <param name="h_points"> text file with Huffman tree points
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readWord2VecFromText(@NonNull File vectors, @NonNull File hs, @NonNull File h_codes, @NonNull File h_points, @NonNull VectorsConfiguration configuration) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readWord2VecFromText(ByVal vectors As File, ByVal hs As File, ByVal h_codes As File, ByVal h_points As File, ByVal configuration As VectorsConfiguration) As Word2Vec
			' first we load syn0
			Dim pair As Pair(Of InMemoryLookupTable, VocabCache) = loadTxt(New FileStream(vectors, FileMode.Open, FileAccess.Read)) 'Note stream is closed in loadTxt
			Dim lookupTable As InMemoryLookupTable = pair.First
			lookupTable.setNegative(configuration.getNegative())
			If configuration.getNegative() > 0 Then
				lookupTable.initNegative()
			End If
			Dim vocab As VocabCache(Of VocabWord) = CType(pair.Second, VocabCache(Of VocabWord))

			' now we load syn1
			Dim reader As New StreamReader(hs)
			Dim line As String = Nothing
			Dim rows As IList(Of INDArray) = New List(Of INDArray)()
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				Dim split() As String = line.Split(" ", True)
				Dim array(split.Length - 1) As Double
				For i As Integer = 0 To split.Length - 1
					array(i) = Double.Parse(split(i))
				Next i
				rows.Add(Nd4j.create(array, New Long(){array.Length}, lookupTable.getSyn0().dataType()))
					line = reader.ReadLine()
			Loop
			reader.Close()

			' it's possible to have full model without syn1
			If rows.Count > 0 Then
				Dim syn1 As INDArray = Nd4j.vstack(rows)
				lookupTable.setSyn1(syn1)
			End If

			' now we transform mappings into huffman tree points
			reader = New StreamReader(h_points)
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				Dim split() As String = line.Split(" ", True)
				Dim word As VocabWord = vocab.wordFor(ReadHelper.decodeB64(split(0)))
				Dim points As IList(Of Integer) = New List(Of Integer)()
				For i As Integer = 1 To split.Length - 1
					points.Add(Integer.Parse(split(i)))
				Next i
				word.setPoints(points)
					line = reader.ReadLine()
			Loop
			reader.Close()


			' now we transform mappings into huffman tree codes
			reader = New StreamReader(h_codes)
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				Dim split() As String = line.Split(" ", True)
				Dim word As VocabWord = vocab.wordFor(ReadHelper.decodeB64(split(0)))
				Dim codes As IList(Of SByte) = New List(Of SByte)()
				For i As Integer = 1 To split.Length - 1
					codes.Add(SByte.Parse(split(i)))
				Next i
				word.Codes = codes
				word.setCodeLength(CShort(codes.Count))
					line = reader.ReadLine()
			Loop
			reader.Close()

			Dim builder As Word2Vec.Builder = (New Word2Vec.Builder(configuration)).vocabCache(vocab).lookupTable(lookupTable).resetModel(False)

			Dim factory As TokenizerFactory = getTokenizerFactory(configuration)

			If factory IsNot Nothing Then
				builder.tokenizerFactory(factory)
			End If

			Dim w2v As Word2Vec = builder.build()

			Return w2v
		End Function


		''' <summary>
		''' Restores previously serialized ParagraphVectors model
		''' <para>
		''' Deprecation note: Please, consider using readParagraphVectors() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="path"> Path to file that contains previously serialized model
		''' @return </param>
		''' @deprecated Use <seealso cref="readParagraphVectors(String)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""readParagraphVectors(String)""/>") public static org.deeplearning4j.models.paragraphvectors.ParagraphVectors readParagraphVectorsFromText(@NonNull String path)
		<Obsolete("Use <seealso cref=""readParagraphVectors(String)""/>")>
		Public Shared Function readParagraphVectorsFromText(ByVal path As String) As ParagraphVectors
			Return readParagraphVectorsFromText(New File(path))
		End Function

		''' <summary>
		''' Restores previously serialized ParagraphVectors model
		''' <para>
		''' Deprecation note: Please, consider using readParagraphVectors() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="file"> File that contains previously serialized model
		''' @return </param>
		''' @deprecated Use <seealso cref="readParagraphVectors(File)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""readParagraphVectors(File)""/>") public static org.deeplearning4j.models.paragraphvectors.ParagraphVectors readParagraphVectorsFromText(@NonNull File file)
		<Obsolete("Use <seealso cref=""readParagraphVectors(File)""/>")>
		Public Shared Function readParagraphVectorsFromText(ByVal file As File) As ParagraphVectors
			Try
					Using fis As New FileStream(file, FileMode.Open, FileAccess.Read)
					Return readParagraphVectorsFromText(fis)
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function


		''' <summary>
		''' Restores previously serialized ParagraphVectors model
		''' <para>
		''' Deprecation note: Please, consider using readParagraphVectors() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="stream"> InputStream that contains previously serialized model </param>
		''' @deprecated Use <seealso cref="readParagraphVectors(InputStream)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""readParagraphVectors(InputStream)""/>") public static org.deeplearning4j.models.paragraphvectors.ParagraphVectors readParagraphVectorsFromText(@NonNull InputStream stream)
		<Obsolete("Use <seealso cref=""readParagraphVectors(InputStream)""/>")>
		Public Shared Function readParagraphVectorsFromText(ByVal stream As Stream) As ParagraphVectors
			Try
				Dim reader As New StreamReader(stream, Encoding.UTF8)
				Dim labels As New List(Of String)()
				Dim arrays As New List(Of INDArray)()
				Dim vocabCache As VocabCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()
				Dim line As String = ""
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					Dim split() As String = line.Split(" ", True)
					split(1) = split(1).replaceAll(WHITESPACE_REPLACEMENT, " ")
					Dim word As New VocabWord(1.0, split(1))
					If split(0).Equals("L") Then
						' we have label element here
						word.setSpecial(True)
						word.markAsLabel(True)
						labels.Add(word.Label)
					ElseIf split(0).Equals("E") Then
						' we have usual element, aka word here
						word.setSpecial(False)
						word.markAsLabel(False)
					Else
						Throw New System.InvalidOperationException("Source stream doesn't looks like ParagraphVectors serialized model")
					End If

					' this particular line is just for backward compatibility with InMemoryLookupCache
					word.Index = vocabCache.numWords()

					vocabCache.addToken(word)
					vocabCache.addWordToIndex(word.Index, word.Label)

					' backward compatibility code
					vocabCache.putVocabWord(word.Label)

					Dim vector(split.Length - 3) As Single

					For i As Integer = 2 To split.Length - 1
						vector(i - 2) = Single.Parse(split(i))
					Next i

					Dim row As INDArray = Nd4j.create(vector)

					arrays.Add(row)
						line = reader.ReadLine()
				Loop

				' now we create syn0 matrix, using previously fetched rows
	'            INDArray syn = Nd4j.create(new int[]{arrays.size(), arrays.get(0).columns()});
	'            for (int i = 0; i < syn.rows(); i++) {
	'                syn.putRow(i, arrays.get(i));
	'            }
				Dim syn As INDArray = Nd4j.vstack(arrays)


				Dim lookupTable As InMemoryLookupTable(Of VocabWord) = CType((New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(arrays(0).columns()).useAdaGrad(False).cache(vocabCache).build(), InMemoryLookupTable(Of VocabWord))
				Nd4j.clearNans(syn)
				lookupTable.setSyn0(syn)

				Dim source As New LabelsSource(labels)
				Dim vectors As ParagraphVectors = (New ParagraphVectors.Builder()).labelsSource(source).vocabCache(vocabCache).lookupTable(lookupTable).modelUtils(New BasicModelUtils(Of VocabWord)()).build()

				Try
					reader.Close()
				Catch e As Exception
				End Try

				vectors.extractLabels()

				Return vectors
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' This method saves paragraph vectors to the given output stream.
		''' </summary>
		''' @deprecated Use <seealso cref="writeParagraphVectors(ParagraphVectors, OutputStream)"/> 
		<Obsolete("Use <seealso cref=""writeParagraphVectors(ParagraphVectors, OutputStream)""/>")>
		Public Shared Sub writeWordVectors(ByVal vectors As ParagraphVectors, ByVal stream As Stream)
			Try
					Using writer As New StreamWriter(stream, Encoding.UTF8)
		'            
		'            This method acts similary to w2v csv serialization, except of additional tag for labels
		'             
        
					Dim vocabCache As VocabCache(Of VocabWord) = vectors.getVocab()
					For Each word As VocabWord In vocabCache.vocabWords()
						Dim builder As New StringBuilder()
        
						builder.Append(If(word.Label, "L", "E")).Append(" ")
						builder.Append(word.Label.replaceAll(" ", WHITESPACE_REPLACEMENT)).Append(" ")
        
						Dim vector As INDArray = vectors.getWordVectorMatrix(word.Label)
						For j As Integer = 0 To vector.length() - 1
							builder.Append(vector.getDouble(j))
							If j < vector.length() - 1 Then
								builder.Append(" ")
							End If
						Next j
        
						writer.Write(builder.Append(vbLf).ToString())
					Next word
					End Using
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' Writes the word vectors to the given path. Note that this assumes an in memory cache
		''' </summary>
		''' <param name="lookupTable"> </param>
		''' <param name="cache"> </param>
		''' <param name="path">        the path to write </param>
		''' <exception cref="IOException"> </exception>
		''' @deprecated Use <seealso cref="writeWord2VecModel(Word2Vec, File)"/> instead 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeWord2VecModel(Word2Vec, File)""/> instead") public static void writeWordVectors(org.deeplearning4j.models.embeddings.inmemory.InMemoryLookupTable lookupTable, org.deeplearning4j.models.word2vec.wordstore.inmemory.InMemoryLookupCache cache, String path) throws java.io.IOException
		<Obsolete("Use <seealso cref=""writeWord2VecModel(Word2Vec, File)""/> instead")>
		Public Shared Sub writeWordVectors(ByVal lookupTable As InMemoryLookupTable, ByVal cache As InMemoryLookupCache, ByVal path As String)
			Using write As New StreamWriter(New FileStream(path, False), Encoding.UTF8)
				Dim i As Integer = 0
				Do While i < lookupTable.getSyn0().rows()
					Dim word As String = cache.wordAtIndex(i)
					If word Is Nothing Then
						i += 1
						Continue Do
					End If
					Dim sb As New StringBuilder()
					sb.Append(word.replaceAll(" ", WHITESPACE_REPLACEMENT))
					sb.Append(" ")
					Dim wordVector As INDArray = lookupTable.vector(word)
					For j As Integer = 0 To wordVector.length() - 1
						sb.Append(wordVector.getDouble(j))
						If j < wordVector.length() - 1 Then
							sb.Append(" ")
						End If
					Next j
					sb.Append(vbLf)
					write.Write(sb.ToString())

					i += 1
				Loop
			End Using
		End Sub

		Private Shared ReadOnly Property ModelMapper As ObjectMapper
			Get
				Dim ret As New ObjectMapper()
				ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
				ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
				ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
				ret.enable(SerializationFeature.INDENT_OUTPUT)
				Return ret
			End Get
		End Property

		''' <summary>
		''' Saves full Word2Vec model in the way, that allows model updates without being rebuilt from scratches
		''' <para>
		''' Deprecation note: Please, consider using writeWord2VecModel() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="vec">  - The Word2Vec instance to be saved </param>
		''' <param name="path"> - the path for json to be saved </param>
		''' @deprecated Use writeWord2VecModel() method instead 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use writeWord2VecModel() method instead") public static void writeFullModel(@NonNull Word2Vec vec, @NonNull String path)
		<Obsolete("Use writeWord2VecModel() method instead")>
		Public Shared Sub writeFullModel(ByVal vec As Word2Vec, ByVal path As String)
	'        
	'            Basically we need to save:
	'                    1. WeightLookupTable, especially syn0 and syn1 matrices
	'                    2. VocabCache, including only WordCounts
	'                    3. Settings from Word2Vect model: workers, layers, etc.
	'         

			Dim printWriter As PrintWriter = Nothing
			Try
				printWriter = New PrintWriter(New StreamWriter(New FileStream(path, FileMode.Create, FileAccess.Write), Encoding.UTF8))
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Dim lookupTable As WeightLookupTable(Of VocabWord) = vec.getLookupTable()
			Dim vocabCache As VocabCache(Of VocabWord) = vec.getVocab() ' ((InMemoryLookupTable) lookupTable).getVocab(); //vec.getVocab();


			If Not (TypeOf lookupTable Is InMemoryLookupTable) Then
				Throw New System.InvalidOperationException("At this moment only InMemoryLookupTable is supported.")
			End If

			Dim conf As VectorsConfiguration = vec.getConfiguration()
			conf.setVocabSize(vocabCache.numWords())


			printWriter.println(conf.toJson())
			'log.info("Word2Vec conf. JSON: " + conf.toJson());
	'        
	'            We have the following map:
	'            Line 0 - VectorsConfiguration JSON string
	'            Line 1 - expTable
	'            Line 2 - table
	'        
	'            All following lines are vocab/weight lookup table saved line by line as VocabularyWord JSON representation
	'         

			' actually we don't need expTable, since it produces exact results on subsequent runs untill you dont modify expTable size :)
			' saving ExpTable just for "special case in future"
			Dim builder As New StringBuilder()
			Dim x As Integer = 0
			Do While x < CType(lookupTable, InMemoryLookupTable).getExpTable().length
				builder.Append(CType(lookupTable, InMemoryLookupTable).getExpTable()(x)).Append(" ")
				x += 1
			Loop
			printWriter.println(builder.ToString().Trim())

			' saving table, available only if negative sampling is used
			If conf.getNegative() > 0 AndAlso CType(lookupTable, InMemoryLookupTable).getTable() IsNot Nothing Then
				builder = New StringBuilder()
				x = 0
				Do While x < CType(lookupTable, InMemoryLookupTable).getTable().columns()
					builder.Append(CType(lookupTable, InMemoryLookupTable).getTable().getDouble(x)).Append(" ")
					x += 1
				Loop
				printWriter.println(builder.ToString().Trim())
			Else
				printWriter.println("")
			End If


			Dim words As IList(Of VocabWord) = New List(Of VocabWord)(vocabCache.vocabWords())
			For Each word As SequenceElement In words
				Dim vw As New VocabularyWord(word.Label)
				vw.setCount(vocabCache.wordFrequency(word.Label))

				vw.setHuffmanNode(VocabularyHolder.buildNode(word.getCodes(), word.getPoints(), word.getCodeLength(), word.Index))


				' writing down syn0
				Dim syn0 As INDArray = CType(lookupTable, InMemoryLookupTable).getSyn0().getRow(vocabCache.indexOf(word.Label))
				Dim dsyn0(syn0.columns() - 1) As Double
				x = 0
				Do While x < conf.getLayersSize()
					dsyn0(x) = syn0.getDouble(x)
					x += 1
				Loop
				vw.setSyn0(dsyn0)

				' writing down syn1
				Dim syn1 As INDArray = CType(lookupTable, InMemoryLookupTable).getSyn1().getRow(vocabCache.indexOf(word.Label))
				Dim dsyn1(syn1.columns() - 1) As Double
				x = 0
				Do While x < syn1.columns()
					dsyn1(x) = syn1.getDouble(x)
					x += 1
				Loop
				vw.setSyn1(dsyn1)

				' writing down syn1Neg, if negative sampling is used
				If conf.getNegative() > 0 AndAlso CType(lookupTable, InMemoryLookupTable).getSyn1Neg() IsNot Nothing Then
					Dim syn1Neg As INDArray = CType(lookupTable, InMemoryLookupTable).getSyn1Neg().getRow(vocabCache.indexOf(word.Label))
					Dim dsyn1Neg(syn1Neg.columns() - 1) As Double
					x = 0
					Do While x < syn1Neg.columns()
						dsyn1Neg(x) = syn1Neg.getDouble(x)
						x += 1
					Loop
					vw.setSyn1Neg(dsyn1Neg)
				End If


				' in case of UseAdaGrad == true - we should save gradients for each word in vocab
				If conf.isUseAdaGrad() AndAlso CType(lookupTable, InMemoryLookupTable).isUseAdaGrad() Then
					Dim gradient As INDArray = word.HistoricalGradient
					If gradient Is Nothing Then
						gradient = Nd4j.zeros(word.getCodes().Count)
					End If
					Dim ada(gradient.columns() - 1) As Double
					x = 0
					Do While x < gradient.columns()
						ada(x) = gradient.getDouble(x)
						x += 1
					Loop
					vw.setHistoricalGradient(ada)
				End If

				printWriter.println(vw.toJson())
			Next word

			' at this moment we have whole vocab serialized
			printWriter.flush()
			printWriter.close()
		End Sub

		''' <summary>
		''' This method loads full w2v model, previously saved with writeFullMethod call
		''' <para>
		''' Deprecation note: Please, consider using readWord2VecModel() or loadStaticModel() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="path"> - path to previously stored w2v json model </param>
		''' <returns> - Word2Vec instance </returns>
		''' @deprecated Use readWord2VecModel() or loadStaticModel() method instead 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use readWord2VecModel() or loadStaticModel() method instead") public static org.deeplearning4j.models.word2vec.Word2Vec loadFullModel(@NonNull String path) throws java.io.FileNotFoundException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		<Obsolete("Use readWord2VecModel() or loadStaticModel() method instead")>
		Public Shared Function loadFullModel(ByVal path As String) As Word2Vec
	'        
	'            // TODO: implementation is in process
	'            We need to restore:
	'                     1. WeightLookupTable, including syn0 and syn1 matrices
	'                     2. VocabCache + mark it as SPECIAL, to avoid accidental word removals
	'         
			Dim iterator As New BasicLineIterator(New File(path))

			' first 3 lines should be processed separately
			Dim confJson As String = iterator.nextSentence()
			log.info("Word2Vec conf. JSON: " & confJson)
			Dim configuration As VectorsConfiguration = VectorsConfiguration.fromJson(confJson)


			' actually we dont need expTable, since it produces exact results on subsequent runs untill you dont modify expTable size :)
			Dim eTable As String = iterator.nextSentence()
			Dim expTable() As Double


			Dim nTable As String = iterator.nextSentence()
			If configuration.getNegative() > 0 Then
				' TODO: we probably should parse negTable, but it's not required until vocab changes are introduced. Since on the predefined vocab it will produce exact nTable, the same goes for expTable btw.
			End If

	'        
	'                Since we're restoring vocab from previously serialized model, we can expect minWordFrequency appliance in its vocabulary, so it should NOT be truncated.
	'                That's why i'm setting minWordFrequency to configuration value, but applying SPECIAL to each word, to avoid truncation
	'         
			Dim holder As VocabularyHolder = (New VocabularyHolder.Builder()).minWordFrequency(configuration.getMinWordFrequency()).hugeModelExpected(configuration.isHugeModelExpected()).scavengerActivationThreshold(configuration.getScavengerActivationThreshold()).scavengerRetentionDelay(configuration.getScavengerRetentionDelay()).build()

			Dim counter As New AtomicInteger(0)
			Dim vocabCache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()
			Do While iterator.hasNext()
				'    log.info("got line: " + iterator.nextSentence());
				Dim wordJson As String = iterator.nextSentence()
				Dim word As VocabularyWord = VocabularyWord.fromJson(wordJson)
				word.setSpecial(True)

				Dim vw As New VocabWord(word.getCount(), word.getWord())
				vw.Index = counter.getAndIncrement()

				vw.Index = word.getHuffmanNode().getIdx()
				vw.setCodeLength(word.getHuffmanNode().getLength())
				vw.setPoints(arrayToList(word.getHuffmanNode().getPoint(), word.getHuffmanNode().getLength()))
				vw.Codes = arrayToList(word.getHuffmanNode().getCode(), word.getHuffmanNode().getLength())

				vocabCache.addToken(vw)
				vocabCache.addWordToIndex(vw.Index, vw.Label)
				vocabCache.putVocabWord(vw.Word)
			Loop

			' at this moment vocab is restored, and it's time to rebuild Huffman tree
			' since word counters are equal, huffman tree will be equal too
			'holder.updateHuffmanCodes();

			' we definitely don't need UNK word in this scenarion


			'        holder.transferBackToVocabCache(vocabCache, false);

			' now, it's time to transfer syn0/syn1/syn1 neg values
			Dim lookupTable As InMemoryLookupTable = CType((New InMemoryLookupTable.Builder()).negative(configuration.getNegative()).useAdaGrad(configuration.isUseAdaGrad()).lr(configuration.getLearningRate()).cache(vocabCache).vectorLength(configuration.getLayersSize()).build(), InMemoryLookupTable)

			' we create all arrays
			lookupTable.resetWeights(True)

			iterator.reset()

			' we should skip 3 lines from file
			iterator.nextSentence()
			iterator.nextSentence()
			iterator.nextSentence()

			' now, for each word from vocabHolder we'll just transfer actual values
			Do While iterator.hasNext()
				Dim wordJson As String = iterator.nextSentence()
				Dim word As VocabularyWord = VocabularyWord.fromJson(wordJson)

				' syn0 transfer
				Dim syn0 As INDArray = lookupTable.getSyn0().getRow(vocabCache.indexOf(word.getWord()))
				syn0.assign(Nd4j.create(word.getSyn0()))

				' syn1 transfer
				' syn1 values are being accessed via tree points, but since our goal is just deserialization - we can just push it row by row
				Dim syn1 As INDArray = lookupTable.getSyn1().getRow(vocabCache.indexOf(word.getWord()))
				syn1.assign(Nd4j.create(word.getSyn1()))

				' syn1Neg transfer
				If configuration.getNegative() > 0 Then
					Dim syn1Neg As INDArray = lookupTable.getSyn1Neg().getRow(vocabCache.indexOf(word.getWord()))
					syn1Neg.assign(Nd4j.create(word.getSyn1Neg()))
				End If
			Loop

			Dim vec As Word2Vec = (New Word2Vec.Builder(configuration)).vocabCache(vocabCache).lookupTable(lookupTable).resetModel(False).build()

			vec.ModelUtils = New BasicModelUtils()

			Return vec
		End Function

		''' <summary>
		''' Writes the word vectors to the given path. Note that this assumes an in memory cache
		''' </summary>
		''' <param name="vec">  the word2vec to write </param>
		''' <param name="path"> the path to write </param>
		''' @deprecated Use <seealso cref="writeWord2VecModel(Word2Vec, String)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeWord2VecModel(Word2Vec, String)""/>") public static void writeWordVectors(@NonNull Word2Vec vec, @NonNull String path) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		<Obsolete("Use <seealso cref=""writeWord2VecModel(Word2Vec, String)""/>")>
		Public Shared Sub writeWordVectors(ByVal vec As Word2Vec, ByVal path As String)
			Dim write As New StreamWriter(New FileStream(New File(path), False), Encoding.UTF8)

			writeWordVectors(vec, write)

			write.Flush()
			write.Close()
		End Sub

		''' <summary>
		''' Writes the word vectors to the given path. Note that this assumes an in memory cache
		''' </summary>
		''' <param name="vec">  the word2vec to write </param>
		''' <param name="file"> the file to write </param>
		''' @deprecated Use <seealso cref="writeWord2VecModel(Word2Vec, File)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeWord2VecModel(Word2Vec, File)""/>") public static void writeWordVectors(@NonNull Word2Vec vec, @NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		<Obsolete("Use <seealso cref=""writeWord2VecModel(Word2Vec, File)""/>")>
		Public Shared Sub writeWordVectors(ByVal vec As Word2Vec, ByVal file As File)
			Using write As New StreamWriter(New FileStream(file, FileMode.Create, FileAccess.Write), Encoding.UTF8)
				writeWordVectors(vec, write)
			End Using
		End Sub

		''' <summary>
		''' Writes the word vectors to the given OutputStream. Note that this assumes an in memory cache.
		''' </summary>
		''' <param name="vec">          the word2vec to write </param>
		''' <param name="outputStream"> - OutputStream, where all data should be sent to
		'''                     the path to write </param>
		''' @deprecated Use <seealso cref="writeWord2Vec(Word2Vec, OutputStream)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeWord2Vec(Word2Vec, OutputStream)""/>") public static void writeWordVectors(@NonNull Word2Vec vec, @NonNull OutputStream outputStream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		<Obsolete("Use <seealso cref=""writeWord2Vec(Word2Vec, OutputStream)""/>")>
		Public Shared Sub writeWordVectors(ByVal vec As Word2Vec, ByVal outputStream As Stream)
			Using writer As New StreamWriter(outputStream, Encoding.UTF8)
				writeWordVectors(vec, writer)
			End Using
		End Sub

		''' <summary>
		''' Writes the word vectors to the given BufferedWriter. Note that this assumes an in memory cache.
		''' BufferedWriter can be writer to local file, or hdfs file, or any compatible to java target.
		''' </summary>
		''' <param name="vec">    the word2vec to write </param>
		''' <param name="writer"> - BufferedWriter, where all data should be written to
		'''               the path to write </param>
		''' @deprecated Use <seealso cref="writeWord2Vec(Word2Vec, OutputStream)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""writeWord2Vec(Word2Vec, OutputStream)""/>") public static void writeWordVectors(@NonNull Word2Vec vec, @NonNull BufferedWriter writer) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		<Obsolete("Use <seealso cref=""writeWord2Vec(Word2Vec, OutputStream)""/>")>
		Public Shared Sub writeWordVectors(ByVal vec As Word2Vec, ByVal writer As StreamWriter)
			Dim words As Integer = 0

			Dim str As String = vec.getVocab().numWords() & " " & vec.getLayerSize() & " " & vec.getVocab().totalNumberOfDocs()
			log.debug("Saving header: {}", str)
			writer.write(str & vbLf)

			For Each word As String In vec.vocab().words()
				If word Is Nothing Then
					Continue For
				End If
				Dim sb As New StringBuilder()
				sb.Append(word.replaceAll(" ", WHITESPACE_REPLACEMENT))
				sb.Append(" ")
				Dim wordVector As INDArray = vec.getWordVectorMatrix(word)
				For j As Integer = 0 To wordVector.length() - 1
					sb.Append(wordVector.getDouble(j))
					If j < wordVector.length() - 1 Then
						sb.Append(" ")
					End If
				Next j
				sb.Append(vbLf)
				writer.write(sb.ToString())
				words += 1
			Next word

			Try
				writer.flush()
			Catch e As Exception
			End Try
			log.info("Wrote " & words & " with size of " & vec.lookupTable().layerSize())
		End Sub

		''' <summary>
		''' Load word vectors for the given vocab and table
		''' </summary>
		''' <param name="table"> the weights to use </param>
		''' <param name="vocab"> the vocab to use </param>
		''' <returns> wordvectors based on the given parameters </returns>
		Public Shared Function fromTableAndVocab(ByVal table As WeightLookupTable, ByVal vocab As VocabCache) As WordVectors
			Dim vectors As New WordVectorsImpl()
			vectors.setLookupTable(table)
			vectors.setVocab(vocab)
			vectors.setModelUtils(New BasicModelUtils())
			Return vectors
		End Function

		''' <summary>
		''' Load word vectors from the given pair
		''' </summary>
		''' <param name="pair"> the given pair </param>
		''' <returns> a read only word vectors impl based on the given lookup table and vocab </returns>
		Public Shared Function fromPair(ByVal pair As Pair(Of InMemoryLookupTable, VocabCache)) As Word2Vec
			Dim vectors As New Word2Vec()
			vectors.LookupTable = pair.First
			vectors.Vocab = pair.Second
			vectors.ModelUtils = New BasicModelUtils()
			Return vectors
		End Function

		''' <summary>
		''' Loads an in memory cache from the given path (sets syn0 and the vocab)
		''' <para>
		''' Deprecation note: Please, consider using readWord2VecModel() or loadStaticModel() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="vectorsFile"> the path of the file to load\
		''' @return </param>
		''' <exception cref="FileNotFoundException"> if the file does not exist </exception>
		''' @deprecated Use <seealso cref="loadTxt(InputStream)"/> 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""loadTxt(InputStream)""/>") public static org.deeplearning4j.models.embeddings.wordvectors.WordVectors loadTxtVectors(java.io.File vectorsFile) throws java.io.IOException
		<Obsolete("Use <seealso cref=""loadTxt(InputStream)""/>")>
		Public Shared Function loadTxtVectors(ByVal vectorsFile As File) As WordVectors
			Dim fileInputStream As New FileStream(vectorsFile, FileMode.Open, FileAccess.Read) 'Note stream is closed in loadTxt
			Dim pair As Pair(Of InMemoryLookupTable, VocabCache) = loadTxt(fileInputStream)
			Return fromPair(pair)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: static java.io.InputStream fileStream(@NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Shared Function fileStream(ByVal file As File) As Stream
			Dim isZip As Boolean = file.getName().EndsWith(".zip")
			Dim isGzip As Boolean = GzipUtils.isCompressedFilename(file.getName())

			Dim inputStream As Stream

			If isZip Then
				inputStream = decompressZip(file)
			ElseIf isGzip Then
				Dim fis As New FileStream(file, FileMode.Open, FileAccess.Read)
				inputStream = New GZIPInputStream(fis)
			Else
				inputStream = New FileStream(file, FileMode.Open, FileAccess.Read)
			End If

			Return New BufferedInputStream(inputStream)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.io.InputStream decompressZip(java.io.File modelFile) throws java.io.IOException
		Private Shared Function decompressZip(ByVal modelFile As File) As Stream
			Dim zipFile As New ZipFile(modelFile)
			Dim inputStream As Stream = Nothing

			Using fis As New FileStream(modelFile, FileMode.Open, FileAccess.Read), bis As New java.io.BufferedInputStream(fis), zipStream As New java.util.zip.ZipInputStream(bis)
				Dim entry As ZipEntry
				entry = zipStream.getNextEntry()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if ((entry = zipStream.getNextEntry()) != null)
				If entry IsNot Nothing Then
					inputStream = zipFile.getInputStream(entry)
				End If

				If zipStream.getNextEntry() IsNot Nothing Then
					Throw New Exception("Zip archive " & modelFile & " contains more than 1 file")
				End If
			End Using

			Return inputStream
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.models.embeddings.inmemory.InMemoryLookupTable, org.deeplearning4j.models.word2vec.wordstore.VocabCache> loadTxt(@NonNull File file)
		Public Shared Function loadTxt(ByVal file As File) As Pair(Of InMemoryLookupTable, VocabCache)
			Try
					Using inputStream As Stream = fileStream(file)
					Return loadTxt(inputStream)
					End Using
			Catch readTestException As IOException
				Throw New Exception(readTestException)
			End Try
		End Function

		''' <summary>
		''' Loads an in memory cache from the given input stream (sets syn0 and the vocab).
		''' </summary>
		''' <param name="inputStream">  input stream </param>
		''' <returns> a <seealso cref="Pair"/> holding the lookup table and the vocab cache. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.models.embeddings.inmemory.InMemoryLookupTable, org.deeplearning4j.models.word2vec.wordstore.VocabCache> loadTxt(@NonNull InputStream inputStream)
		Public Shared Function loadTxt(ByVal inputStream As Stream) As Pair(Of InMemoryLookupTable, VocabCache)
			Dim cache As New AbstractCache(Of VocabWord)()
			Dim lines As LineIterator = Nothing

			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using inputStreamReader As System.IO.StreamReader_OutputStreamReader = new System.IO.StreamReader(inputStream), reader As System.IO.StreamReader_BufferedReader = new System.IO.StreamReader_BufferedReader(inputStreamReader)
					New StreamReader(inputStream), reader As New StreamReader(inputStreamReader)
						Using inputStreamReader As New StreamReader(inputStream), reader As StreamReader
					lines = IOUtils.lineIterator(reader)
        
					Dim line As String = Nothing
					Dim hasHeader As Boolean = False
        
					' Check if first line is a header 
					If lines.hasNext() Then
						line = lines.nextLine()
						hasHeader = isHeader(line, cache)
					End If
        
					If hasHeader Then
						log.debug("First line is a header")
						line = lines.nextLine()
					End If
        
					Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
					Dim vShape() As Long = { 1, -1 }
        
					Do
						Dim tokens() As String = line.Split(" ", True)
						Dim word As String = ReadHelper.decodeB64(tokens(0))
						Dim vocabWord As New VocabWord(1.0, word)
						vocabWord.Index = cache.numWords()
        
						cache.addToken(vocabWord)
						cache.addWordToIndex(vocabWord.Index, word)
						cache.putVocabWord(word)
        
						Dim vector(tokens.Length - 2) As Single
						For i As Integer = 1 To tokens.Length - 1
							vector(i - 1) = Single.Parse(tokens(i))
						Next i
        
						vShape(1) = vector.Length
						Dim row As INDArray = Nd4j.create(vector, vShape)
        
						arrays.Add(row)
        
						line = If(lines.hasNext(), lines.next(), Nothing)
					Loop While line IsNot Nothing
        
					Dim syn As INDArray = Nd4j.vstack(arrays)
        
					Dim lookupTable As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(arrays(0).columns()).useAdaGrad(False).cache(cache).useHierarchicSoftmax(False).build()
        
					lookupTable.setSyn0(syn)
        
					Return New Pair(Of InMemoryLookupTable, VocabCache)(CType(lookupTable, InMemoryLookupTable), CType(cache, VocabCache))
					End Using
			Catch readeTextStreamException As IOException
				Throw New Exception(readeTextStreamException)
			Finally
				If lines IsNot Nothing Then
					lines.close()
				End If
			End Try
		End Function

		Friend Shared Function isHeader(ByVal line As String, ByVal cache As AbstractCache) As Boolean
			If Not line.Contains(" ") Then
				Return True
			Else

	'             We should check for something that looks like proper word vectors here. i.e: 1 word at the 0
	'             * position, and bunch of floats further 
				Dim headers() As String = line.Split(" ", True)

				Try
					Dim header(headers.Length - 1) As Long
					For x As Integer = 0 To headers.Length - 1
						header(x) = Long.Parse(headers(x))
					Next x

	'                 Now we know, if that's all ints - it's just a header
	'                 * [0] - number of words
	'                 * [1] - vectorLength
	'                 * [2] - number of documents <-- DL4j-only value
	'                 
					If headers.Length = 3 Then
						Dim numberOfDocuments As Long = header(2)
						cache.incrementTotalDocCount(numberOfDocuments)
					End If

					Dim numWords As Long = header(0)
					Dim vectorLength As Integer = CInt(header(1))
					printOutProjectedMemoryUse(numWords, vectorLength, 1)

					Return True
				Catch notHeaderException As Exception
					' if any conversion exception hits - that'll be considered header
					Return False
				End Try
			End If
		End Function

		''' <summary>
		''' This method can be used to load previously saved model from InputStream (like a HDFS-stream)
		''' <para>
		''' Deprecation note: Please, consider using readWord2VecModel() or loadStaticModel() method instead
		''' 
		''' </para>
		''' </summary>
		''' <param name="stream">        InputStream that contains previously serialized model </param>
		''' <param name="skipFirstLine"> Set this TRUE if first line contains csv header, FALSE otherwise
		''' @return </param>
		''' <exception cref="IOException"> </exception>
		''' @deprecated Use readWord2VecModel() or loadStaticModel() method instead 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use readWord2VecModel() or loadStaticModel() method instead") public static org.deeplearning4j.models.embeddings.wordvectors.WordVectors loadTxtVectors(@NonNull InputStream stream, boolean skipFirstLine) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		<Obsolete("Use readWord2VecModel() or loadStaticModel() method instead")>
		Public Shared Function loadTxtVectors(ByVal stream As Stream, ByVal skipFirstLine As Boolean) As WordVectors
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim reader As New StreamReader(stream)
			Dim line As String = ""
			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()

			If skipFirstLine Then
				reader.ReadLine()
			End If

			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				Dim split() As String = line.Split(" ", True)
				Dim word As String = split(0).replaceAll(WHITESPACE_REPLACEMENT, " ")
				Dim word1 As New VocabWord(1.0, word)

				word1.Index = cache.numWords()

				cache.addToken(word1)

				cache.addWordToIndex(word1.Index, word)

				cache.putVocabWord(word)

				Dim vector(split.Length - 2) As Single

				For i As Integer = 1 To split.Length - 1
					vector(i - 1) = Single.Parse(split(i))
				Next i

				Dim row As INDArray = Nd4j.create(vector)

				arrays.Add(row)
					line = reader.ReadLine()
			Loop

			Dim lookupTable As InMemoryLookupTable(Of VocabWord) = CType((New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(arrays(0).columns()).cache(cache).build(), InMemoryLookupTable(Of VocabWord))

			Dim syn As INDArray = Nd4j.vstack(arrays)

			Nd4j.clearNans(syn)
			lookupTable.setSyn0(syn)

			Return fromPair(Pair.makePair(CType(lookupTable, InMemoryLookupTable), CType(cache, VocabCache)))
		End Function

		''' <summary>
		''' Write the tsne format
		''' </summary>
		''' <param name="vec">  the word vectors to use for labeling </param>
		''' <param name="tsne"> the tsne array to write </param>
		''' <param name="csv">  the file to use </param>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeTsneFormat(org.deeplearning4j.models.word2vec.Word2Vec vec, org.nd4j.linalg.api.ndarray.INDArray tsne, java.io.File csv) throws Exception
		Public Shared Sub writeTsneFormat(ByVal vec As Word2Vec, ByVal tsne As INDArray, ByVal csv As File)
			Using write As New StreamWriter(New FileStream(csv, FileMode.Create, FileAccess.Write), Encoding.UTF8)
				Dim words As Integer = 0
				Dim l As InMemoryLookupCache = CType(vec.vocab(), InMemoryLookupCache)
				For Each word As String In vec.vocab().words()
					If word Is Nothing Then
						Continue For
					End If
					Dim sb As New StringBuilder()
					Dim wordVector As INDArray = tsne.getRow(l.wordFor(word).Index)
					For j As Integer = 0 To wordVector.length() - 1
						sb.Append(wordVector.getDouble(j))
						If j < wordVector.length() - 1 Then
							sb.Append(",")
						End If
					Next j
					sb.Append(",")
					sb.Append(word.replaceAll(" ", WHITESPACE_REPLACEMENT))
					sb.Append(" ")

					sb.Append(vbLf)
					write.Write(sb.ToString())

				Next word

				log.info("Wrote " & words & " with size of " & vec.lookupTable().layerSize())
			End Using
		End Sub


		''' <summary>
		''' This method is used only for VocabCache compatibility purposes
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="codeLen">
		''' @return </param>
		Private Shared Function arrayToList(ByVal array() As SByte, ByVal codeLen As Integer) As IList(Of SByte)
			Dim result As IList(Of SByte) = New List(Of SByte)()
			For x As Integer = 0 To codeLen - 1
				result.Add(array(x))
			Next x
			Return result
		End Function

		Private Shared Function listToArray(ByVal code As IList(Of SByte)) As SByte()
			Dim array(39) As SByte
			For x As Integer = 0 To code.Count - 1
				array(x) = code(x).byteValue()
			Next x
			Return array
		End Function

		Private Shared Function listToArray(ByVal points As IList(Of Integer), ByVal codeLen As Integer) As Integer()
			Dim array(points.Count - 1) As Integer
			For x As Integer = 0 To points.Count - 1
				array(x) = points(x).intValue()
			Next x
			Return array
		End Function

		''' <summary>
		''' This method is used only for VocabCache compatibility purposes
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="codeLen">
		''' @return </param>
		Private Shared Function arrayToList(ByVal array() As Integer, ByVal codeLen As Integer) As IList(Of Integer)
			Dim result As IList(Of Integer) = New List(Of Integer)()
			For x As Integer = 0 To codeLen - 1
				result.Add(array(x))
			Next x
			Return result
		End Function

		''' <summary>
		''' This method saves specified SequenceVectors model to target  file path
		''' </summary>
		''' <param name="vectors"> SequenceVectors model </param>
		''' <param name="factory"> SequenceElementFactory implementation for your objects </param>
		''' <param name="path">    Target output file path </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeSequenceVectors(@NonNull SequenceVectors<T> vectors, @NonNull SequenceElementFactory<T> factory, @NonNull String path) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeSequenceVectors(Of T As SequenceElement)(ByVal vectors As SequenceVectors(Of T), ByVal factory As SequenceElementFactory(Of T), ByVal path As String)
			Using fos As New FileStream(path, FileMode.Create, FileAccess.Write)
				writeSequenceVectors(vectors, factory, fos)
			End Using
		End Sub

		''' <summary>
		''' This method saves specified SequenceVectors model to target  file
		''' </summary>
		''' <param name="vectors"> SequenceVectors model </param>
		''' <param name="factory"> SequenceElementFactory implementation for your objects </param>
		''' <param name="file">    Target output file </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeSequenceVectors(@NonNull SequenceVectors<T> vectors, @NonNull SequenceElementFactory<T> factory, @NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeSequenceVectors(Of T As SequenceElement)(ByVal vectors As SequenceVectors(Of T), ByVal factory As SequenceElementFactory(Of T), ByVal file As File)
			Using fos As New FileStream(file, FileMode.Create, FileAccess.Write)
				writeSequenceVectors(vectors, factory, fos)
			End Using
		End Sub

		''' <summary>
		''' This method saves specified SequenceVectors model to target  OutputStream
		''' </summary>
		''' <param name="vectors"> SequenceVectors model </param>
		''' <param name="factory"> SequenceElementFactory implementation for your objects </param>
		''' <param name="stream">  Target output stream </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeSequenceVectors(@NonNull SequenceVectors<T> vectors, @NonNull SequenceElementFactory<T> factory, @NonNull OutputStream stream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeSequenceVectors(Of T As SequenceElement)(ByVal vectors As SequenceVectors(Of T), ByVal factory As SequenceElementFactory(Of T), ByVal stream As Stream)
			Dim lookupTable As WeightLookupTable(Of T) = vectors.getLookupTable()
			Dim vocabCache As VocabCache(Of T) = vectors.getVocab()

			Using writer As New java.io.PrintWriter(New StreamWriter(stream, Encoding.UTF8))

				' at first line we save VectorsConfiguration
				writer.write(vectors.getConfiguration().toEncodedJson())

				' now we have elements one by one
				Dim x As Integer = 0
				Do While x < vocabCache.numWords()
					Dim element As T = vocabCache.elementAtIndex(x)
					Dim json As String = factory.serialize(element)
					Dim d As INDArray = Nd4j.create(1)
					Dim vector() As Double = lookupTable.vector(element.getLabel()).dup().data().asDouble()
					Dim pair As New ElementPair(json, vector)
					writer.println(pair.toEncodedJson())
					writer.flush()
					x += 1
				Loop
			End Using
		End Sub

		Private Const CONFIG_ENTRY As String = "config.json"
		Private Const VOCAB_ENTRY As String = "vocabulary.json"
		Private Const SYN0_ENTRY As String = "syn0.bin"
		Private Const SYN1_ENTRY As String = "syn1.bin"
		Private Const SYN1_NEG_ENTRY As String = "syn1neg.bin"

		''' <summary>
		''' This method saves specified SequenceVectors model to target  OutputStream
		''' </summary>
		''' <param name="vectors"> SequenceVectors model </param>
		''' <param name="stream">  Target output stream </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeSequenceVectors(@NonNull SequenceVectors<T> vectors, @NonNull OutputStream stream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeSequenceVectors(Of T As SequenceElement)(ByVal vectors As SequenceVectors(Of T), ByVal stream As Stream)

			Dim lookupTable As InMemoryLookupTable(Of VocabWord) = CType(vectors.getLookupTable(), InMemoryLookupTable(Of VocabWord))
			Dim vocabCache As AbstractCache(Of T) = CType(vectors.getVocab(), AbstractCache(Of T))

			Using zipfile As New java.util.zip.ZipOutputStream(New java.io.BufferedOutputStream(New org.apache.commons.io.output.CloseShieldOutputStream(stream))), dos As New java.io.DataOutputStream(New java.io.BufferedOutputStream(zipfile))

				Dim config As New ZipEntry(CONFIG_ENTRY)
				zipfile.putNextEntry(config)
				Dim configuration As VectorsConfiguration = vectors.getConfiguration()

				Dim json As String = configuration.toJson().Trim()
				zipfile.write(json.GetBytes(Encoding.UTF8))

				Dim vocab As New ZipEntry(VOCAB_ENTRY)
				zipfile.putNextEntry(vocab)
				zipfile.write(vocabCache.toJson().GetBytes(Encoding.UTF8))

				Dim syn0Data As INDArray = lookupTable.getSyn0()
				Dim syn0 As New ZipEntry(SYN0_ENTRY)
				zipfile.putNextEntry(syn0)
				Nd4j.write(syn0Data, dos)
				dos.flush()

				Dim syn1Data As INDArray = lookupTable.getSyn1()
				If syn1Data IsNot Nothing Then
					Dim syn1 As New ZipEntry(SYN1_ENTRY)
					zipfile.putNextEntry(syn1)
					Nd4j.write(syn1Data, dos)
					dos.flush()
				End If

				Dim syn1NegData As INDArray = lookupTable.Syn1Neg
				If syn1NegData IsNot Nothing Then
					Dim syn1neg As New ZipEntry(SYN1_NEG_ENTRY)
					zipfile.putNextEntry(syn1neg)
					Nd4j.write(syn1NegData, dos)
					dos.flush()
				End If
			End Using
		End Sub

		''' <summary>
		''' This method loads SequenceVectors from specified file path
		''' </summary>
		''' <param name="path"> String </param>
		''' <param name="readExtendedTables"> boolean </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.sequencevectors.SequenceVectors<T> readSequenceVectors(@NonNull String path, boolean readExtendedTables) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readSequenceVectors(Of T As SequenceElement)(ByVal path As String, ByVal readExtendedTables As Boolean) As SequenceVectors(Of T)

			Dim file As New File(path)
			Dim vectors As SequenceVectors(Of T) = readSequenceVectors(file, readExtendedTables)
			Return vectors
		End Function

		''' <summary>
		''' This method loads SequenceVectors from specified file path
		''' </summary>
		''' <param name="file"> File </param>
		''' <param name="readExtendedTables"> boolean </param>
		''' @param <T> </param>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.sequencevectors.SequenceVectors<T> readSequenceVectors(@NonNull File file, boolean readExtendedTables) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readSequenceVectors(Of T As SequenceElement)(ByVal file As File, ByVal readExtendedTables As Boolean) As SequenceVectors(Of T)

			Dim vectors As SequenceVectors(Of T) = readSequenceVectors(New FileStream(file, FileMode.Open, FileAccess.Read), readExtendedTables)
			Return vectors
		End Function

		''' <summary>
		''' This method loads SequenceVectors from specified input stream
		''' </summary>
		''' <param name="stream"> InputStream </param>
		''' <param name="readExtendedTables"> boolean </param>
		''' @param <T> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.sequencevectors.SequenceVectors<T> readSequenceVectors(@NonNull InputStream stream, boolean readExtendedTables) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readSequenceVectors(Of T As SequenceElement)(ByVal stream As Stream, ByVal readExtendedTables As Boolean) As SequenceVectors(Of T)

			Dim vectors As SequenceVectors(Of T) = Nothing
			Dim vocabCache As AbstractCache(Of T) = Nothing
			Dim configuration As VectorsConfiguration = Nothing

			Dim syn0 As INDArray = Nothing, syn1 As INDArray = Nothing, syn1neg As INDArray = Nothing

			Using zipfile As New java.util.zip.ZipInputStream(New java.io.BufferedInputStream(stream))

				Dim entry As ZipEntry = Nothing
				entry = zipfile.getNextEntry()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((entry = zipfile.getNextEntry()) != null)
				Do While entry IsNot Nothing

					Dim name As String = entry.getName()
					Dim bytes() As SByte = IOUtils.toByteArray(zipfile)

					If name.Equals(CONFIG_ENTRY) Then
						Dim content As String = StringHelper.NewString(bytes, "UTF-8")
						configuration = VectorsConfiguration.fromJson(content)
						Continue Do
					ElseIf name.Equals(VOCAB_ENTRY) Then
						Dim content As String = StringHelper.NewString(bytes, "UTF-8")
						vocabCache = AbstractCache.fromJson(content)
						Continue Do
					End If
					If readExtendedTables Then
						If name.Equals(SYN0_ENTRY) Then
							syn0 = Nd4j.read(New MemoryStream(bytes))

						ElseIf name.Equals(SYN1_ENTRY) Then
							syn1 = Nd4j.read(New MemoryStream(bytes))
						ElseIf name.Equals(SYN1_NEG_ENTRY) Then
							syn1neg = Nd4j.read(New MemoryStream(bytes))
						End If
					End If
						entry = zipfile.getNextEntry()
				Loop

			End Using
			Dim lookupTable As New InMemoryLookupTable(Of T)()
			lookupTable.setSyn0(syn0)
			lookupTable.setSyn1(syn1)
			lookupTable.Syn1Neg = syn1neg
			vectors = (New SequenceVectors.Builder(Of T)(configuration)).lookupTable(lookupTable).vocabCache(vocabCache).build()
			Return vectors
		End Function

		''' <summary>
		''' This method loads previously saved SequenceVectors model from File
		''' </summary>
		''' <param name="factory"> </param>
		''' <param name="file"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.sequencevectors.SequenceVectors<T> readSequenceVectors(@NonNull SequenceElementFactory<T> factory, @NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readSequenceVectors(Of T As SequenceElement)(ByVal factory As SequenceElementFactory(Of T), ByVal file As File) As SequenceVectors(Of T)
			Return readSequenceVectors(factory, New FileStream(file, FileMode.Open, FileAccess.Read))
		End Function

		''' <summary>
		''' This method loads previously saved SequenceVectors model from InputStream
		''' </summary>
		''' <param name="factory"> </param>
		''' <param name="stream"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.sequencevectors.SequenceVectors<T> readSequenceVectors(@NonNull SequenceElementFactory<T> factory, @NonNull InputStream stream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readSequenceVectors(Of T As SequenceElement)(ByVal factory As SequenceElementFactory(Of T), ByVal stream As Stream) As SequenceVectors(Of T)
			Dim reader As New StreamReader(stream, Encoding.UTF8)

			' at first we load vectors configuration
			Dim line As String = reader.ReadLine()
			Dim configuration As VectorsConfiguration = VectorsConfiguration.fromJson(New String(Base64.decodeBase64(line), "UTF-8"))

			Dim vocabCache As AbstractCache(Of T) = (New AbstractCache.Builder(Of T)()).build()


			Dim rows As IList(Of INDArray) = New List(Of INDArray)()

			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				If line.Length = 0 Then ' skip empty line
					Continue Do
				End If
				Dim pair As ElementPair = ElementPair.fromEncodedJson(line)
				Dim element As T = factory.deserialize(pair.getObject())
				rows.Add(Nd4j.create(pair.getVector()))
				vocabCache.addToken(element)
				vocabCache.addWordToIndex(element.getIndex(), element.getLabel())
					line = reader.ReadLine()
			Loop

			reader.Close()

			Dim lookupTable As InMemoryLookupTable(Of T) = CType((New InMemoryLookupTable.Builder(Of T)()).vectorLength(rows(0).columns()).cache(vocabCache).build(), InMemoryLookupTable(Of T)) ' fix: add vocab cache

	'        
	'         * INDArray syn0 = Nd4j.create(rows.size(), rows.get(0).columns()); for (int x = 0; x < rows.size(); x++) {
	'         * syn0.putRow(x, rows.get(x)); }
	'         
			Dim syn0 As INDArray = Nd4j.vstack(rows)

			lookupTable.setSyn0(syn0)

			Dim vectors As SequenceVectors(Of T) = (New SequenceVectors.Builder(Of T)(configuration)).vocabCache(vocabCache).lookupTable(lookupTable).resetModel(False).build()

			Return vectors
		End Function

		''' <summary>
		''' This method saves vocab cache to provided File.
		''' Please note: it saves only vocab content, so it's suitable mostly for BagOfWords/TF-IDF vectorizers
		''' </summary>
		''' <param name="vocabCache"> </param>
		''' <param name="file"> </param>
		''' <exception cref="UnsupportedEncodingException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeVocabCache(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, @NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeVocabCache(ByVal vocabCache As VocabCache(Of VocabWord), ByVal file As File)
			Using fos As New FileStream(file, FileMode.Create, FileAccess.Write)
				writeVocabCache(vocabCache, fos)
			End Using
		End Sub

		''' <summary>
		''' This method saves vocab cache to provided OutputStream.
		''' Please note: it saves only vocab content, so it's suitable mostly for BagOfWords/TF-IDF vectorizers
		''' </summary>
		''' <param name="vocabCache"> </param>
		''' <param name="stream"> </param>
		''' <exception cref="UnsupportedEncodingException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeVocabCache(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, @NonNull OutputStream stream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeVocabCache(ByVal vocabCache As VocabCache(Of VocabWord), ByVal stream As Stream)
			Using writer As New java.io.PrintWriter(New StreamWriter(stream, Encoding.UTF8))
				' saving general vocab information
				writer.println("" & vocabCache.numWords() & " " & vocabCache.totalNumberOfDocs() & " " & vocabCache.totalWordOccurrences())

				Dim x As Integer = 0
				Do While x < vocabCache.numWords()
					Dim word As VocabWord = vocabCache.elementAtIndex(x)
					writer.println(word.toJSON())
					x += 1
				Loop
			End Using
		End Sub

		''' <summary>
		''' This method reads vocab cache from provided file.
		''' Please note: it reads only vocab content, so it's suitable mostly for BagOfWords/TF-IDF vectorizers
		''' </summary>
		''' <param name="file">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> readVocabCache(@NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readVocabCache(ByVal file As File) As VocabCache(Of VocabWord)
			Using fis As New FileStream(file, FileMode.Open, FileAccess.Read)
				Return readVocabCache(fis)
			End Using
		End Function

		''' <summary>
		''' This method reads vocab cache from provided InputStream.
		''' Please note: it reads only vocab content, so it's suitable mostly for BagOfWords/TF-IDF vectorizers
		''' </summary>
		''' <param name="stream">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> readVocabCache(@NonNull InputStream stream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readVocabCache(ByVal stream As Stream) As VocabCache(Of VocabWord)
			Dim vocabCache As val = (New AbstractCache.Builder(Of VocabWord)()).build()
			Dim factory As val = New VocabWordFactory()
			Dim firstLine As Boolean = True
			Dim totalWordOcc As Long = -1L
			Using reader As New StreamReader(stream, Encoding.UTF8)
				Dim line As String
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					' try to treat first line as header with 3 digits
					If firstLine Then
						firstLine = False
						Dim split As val = line.Split("\ ", True)

						If split.length <> 3 Then
							Continue Do
						End If

						Try
							vocabCache.setTotalDocCount(Convert.ToInt64(split(1)))
							totalWordOcc = Convert.ToInt64(split(2))
							Continue Do
						Catch e As System.FormatException
							' no-op
						End Try
					End If

					Dim word As val = factory.deserialize(line)

					vocabCache.addToken(word)
					vocabCache.addWordToIndex(word.getIndex(), word.getLabel())
						line = reader.ReadLine()
				Loop
			End Using

			If totalWordOcc >= 0 Then
				vocabCache.setTotalWordOccurences(totalWordOcc)
			End If

			Return vocabCache
		End Function

		''' <summary>
		''' This is utility holder class
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor private static class ElementPair
		Private Class ElementPair
			Friend [object] As String
			Friend vector() As Double

			''' <summary>
			''' This utility method serializes ElementPair into JSON + packs it into Base64-encoded string
			''' 
			''' @return
			''' </summary>
			Protected Friend Overridable Function toEncodedJson() As String
				Dim mapper As ObjectMapper = SequenceElement.mapper()
				Dim base64 As New Base64(Integer.MaxValue)
				Try
					Dim json As String = mapper.writeValueAsString(Me)
					Dim output As String = base64.encodeAsString(json.GetBytes(Encoding.UTF8))
					Return output
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Function

			''' <summary>
			''' This utility method returns ElementPair from Base64-encoded string
			''' </summary>
			''' <param name="encoded">
			''' @return </param>
			Protected Friend Shared Function fromEncodedJson(ByVal encoded As String) As ElementPair
				Dim mapper As ObjectMapper = SequenceElement.mapper()
				Try
					Dim decoded As New String(Base64.decodeBase64(encoded), "UTF-8")
					Return mapper.readValue(decoded, GetType(ElementPair))
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End Function
		End Class

		''' <summary>
		''' This method
		''' 1) Binary model, either compressed or not. Like well-known Google Model
		''' 2) Popular CSV word2vec text format
		''' 3) DL4j compressed format
		''' <para>
		''' Please note: Only weights will be loaded by this method.
		''' 
		''' </para>
		''' </summary>
		''' <param name="path">
		''' @return </param>
		Public Shared Function readWord2VecModel(ByVal path As String) As Word2Vec
			Return readWord2VecModel(New File(path))
		End Function

		''' <summary>
		''' This method
		''' 1) Binary model, either compressed or not. Like well-known Google Model
		''' 2) Popular CSV word2vec text format
		''' 3) DL4j compressed format
		''' <para>
		''' Please note: Only weights will be loaded by this method.
		''' 
		''' </para>
		''' </summary>
		''' <param name="path">  path to model file </param>
		''' <param name="extendedModel">  if TRUE, we'll try to load HS states & Huffman tree info, if FALSE, only weights will be loaded
		''' @return </param>
		Public Shared Function readWord2VecModel(ByVal path As String, ByVal extendedModel As Boolean) As Word2Vec
			Return readWord2VecModel(New File(path), extendedModel)
		End Function

		''' <summary>
		''' This method
		''' 1) Binary model, either compressed or not. Like well-known Google Model
		''' 2) Popular CSV word2vec text format
		''' 3) DL4j compressed format
		''' <para>
		''' Please note: Only weights will be loaded by this method.
		''' 
		''' </para>
		''' </summary>
		''' <param name="file">
		''' @return </param>
		Public Shared Function readWord2VecModel(ByVal file As File) As Word2Vec
			Return readWord2VecModel(file, False)
		End Function

		''' <summary>
		''' This method
		''' 1) Binary model, either compressed or not. Like well-known Google Model
		''' 2) Popular CSV word2vec text format
		''' 3) DL4j compressed format
		''' <para>
		''' Please note: if extended data isn't available, only weights will be loaded instead.
		''' 
		''' </para>
		''' </summary>
		''' <param name="file">  model file </param>
		''' <param name="extendedModel">  if TRUE, we'll try to load HS states & Huffman tree info, if FALSE, only weights will be loaded </param>
		''' <returns> word2vec model </returns>
		Public Shared Function readWord2VecModel(ByVal file As File, ByVal extendedModel As Boolean) As Word2Vec
			If Not file.exists() OrElse Not file.isFile() Then
				Throw New ND4JIllegalStateException("File [" & file.getAbsolutePath() & "] doesn't exist")
			End If

			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive
			If originalPeriodic Then
				Nd4j.MemoryManager.togglePeriodicGc(False)
			End If
			Nd4j.MemoryManager.OccasionalGcFrequency = 50000

			Try
				Return readWord2Vec(file, extendedModel)
			Catch readSequenceVectors As Exception
				Try
					Return If(extendedModel, readAsExtendedModel(file), readAsSimplifiedModel(file))
				Catch loadFromFileException As Exception
					Try
						Return readAsCsv(file)
					Catch readCsvException As Exception
						Try
							Return readAsBinary(file)
						Catch readBinaryException As Exception
							Try
								Return readAsBinaryNoLineBreaks(file)
							Catch readModelException As Exception
								log.error("Unable to guess input file format", readModelException)
								Throw New Exception("Unable to guess input file format. Please use corresponding loader directly")
							End Try
						End Try
					End Try
				End Try
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readAsBinaryNoLineBreaks(@NonNull File file)
		Public Shared Function readAsBinaryNoLineBreaks(ByVal file As File) As Word2Vec
			Try
					Using inputStream As Stream = fileStream(file)
					Return readAsBinaryNoLineBreaks(inputStream)
					End Using
			Catch readCsvException As IOException
				Throw New Exception(readCsvException)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readAsBinaryNoLineBreaks(@NonNull InputStream inputStream)
		Public Shared Function readAsBinaryNoLineBreaks(ByVal inputStream As Stream) As Word2Vec
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive
			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency

			' try to load without linebreaks
			Try
				If originalPeriodic Then
					Nd4j.MemoryManager.togglePeriodicGc(True)
				End If

				Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq

				Return readBinaryModel(inputStream, False, False)
			Catch readModelException As Exception
				log.error("Cannot read binary model", readModelException)
				Throw New Exception("Unable to guess input file format. Please use corresponding loader directly")
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readAsBinary(@NonNull File file)
		Public Shared Function readAsBinary(ByVal file As File) As Word2Vec
			Try
					Using inputStream As Stream = fileStream(file)
					Return readAsBinary(inputStream)
					End Using
			Catch readCsvException As IOException
				Throw New Exception(readCsvException)
			End Try
		End Function

		''' <summary>
		''' This method loads Word2Vec model from binary input stream.
		''' </summary>
		''' <param name="inputStream">  binary input stream </param>
		''' <returns> Word2Vec </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readAsBinary(@NonNull InputStream inputStream)
		Public Shared Function readAsBinary(ByVal inputStream As Stream) As Word2Vec
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive
			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency

			' we fallback to trying binary model instead
			Try
				log.debug("Trying binary model restoration...")

				If originalPeriodic Then
					Nd4j.MemoryManager.togglePeriodicGc(True)
				End If

				Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq

				Return readBinaryModel(inputStream, True, False)
			Catch readModelException As Exception
				Throw New Exception(readModelException)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readAsCsv(@NonNull File file)
		Public Shared Function readAsCsv(ByVal file As File) As Word2Vec
			Try
					Using inputStream As Stream = fileStream(file)
					Return readAsCsv(inputStream)
					End Using
			Catch readCsvException As IOException
				Throw New Exception(readCsvException)
			End Try
		End Function

		''' <summary>
		''' This method loads Word2Vec model from csv file
		''' </summary>
		''' <param name="inputStream">  input stream </param>
		''' <returns> Word2Vec model </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readAsCsv(@NonNull InputStream inputStream)
		Public Shared Function readAsCsv(ByVal inputStream As Stream) As Word2Vec
			Dim configuration As New VectorsConfiguration()

			' let's try to load this file as csv file
			Try
				log.debug("Trying CSV model restoration...")

				Dim pair As Pair(Of InMemoryLookupTable, VocabCache) = loadTxt(inputStream)
				Dim builder As Word2Vec.Builder = (New Word2Vec.Builder()).lookupTable(pair.First).useAdaGrad(False).vocabCache(pair.Second).layerSize(pair.First.layerSize()).useHierarchicSoftmax(False).resetModel(False)

				Dim factory As TokenizerFactory = getTokenizerFactory(configuration)
				If factory IsNot Nothing Then
					builder.tokenizerFactory(factory)
				End If

				Return builder.build()
			Catch ex As Exception
				Throw New Exception("Unable to load model in CSV format")
			End Try
		End Function

		''' <summary>
		''' This method just loads full compressed model.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.deeplearning4j.models.word2vec.Word2Vec readAsExtendedModel(@NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Private Shared Function readAsExtendedModel(ByVal file As File) As Word2Vec
			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive

			log.debug("Trying full model restoration...")

			If originalPeriodic Then
				Nd4j.MemoryManager.togglePeriodicGc(True)
			End If

			Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq

			Return readWord2Vec(file)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.deeplearning4j.models.word2vec.Word2Vec readAsSimplifiedModel(@NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Private Shared Function readAsSimplifiedModel(ByVal file As File) As Word2Vec
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive
			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency
			Dim lookupTable As New InMemoryLookupTable(Of VocabWord)()
			Dim vocabCache As New AbstractCache(Of VocabWord)()
			Dim vec As Word2Vec
			Dim syn0 As INDArray = Nothing
			Dim configuration As New VectorsConfiguration()

			log.debug("Trying simplified model restoration...")

			Dim tmpFileSyn0 As File = DL4JFileUtils.createTempFile("word2vec", "syn")
			tmpFileSyn0.deleteOnExit()
			Dim tmpFileConfig As File = DL4JFileUtils.createTempFile("word2vec", "config")
			tmpFileConfig.deleteOnExit()
			' we don't need full model, so we go directly to syn0 file

			Dim zipFile As New ZipFile(file)
			Dim syn As ZipEntry = zipFile.getEntry("syn0.txt")
			Dim stream As Stream = zipFile.getInputStream(syn)

			FileUtils.copyInputStreamToFile(stream, tmpFileSyn0)

			' now we're restoring configuration saved earlier
			Dim config As ZipEntry = zipFile.getEntry("config.json")
			If config IsNot Nothing Then
				stream = zipFile.getInputStream(config)

				Dim builder As New StringBuilder()
				Using reader As New StreamReader(stream)
					Dim line As String
					line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
					Do While line IsNot Nothing
						builder.Append(line)
							line = reader.ReadLine()
					Loop
				End Using

				configuration = VectorsConfiguration.fromJson(builder.ToString().Trim())
			End If

			Dim ve As ZipEntry = zipFile.getEntry("frequencies.txt")
			If ve IsNot Nothing Then
				stream = zipFile.getInputStream(ve)
				Dim cnt As New AtomicInteger(0)
				Using reader As New StreamReader(stream)
					Dim line As String
					line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
					Do While line IsNot Nothing
						Dim split() As String = line.Split(" ", True)
						Dim word As New VocabWord(Convert.ToDouble(split(1)), ReadHelper.decodeB64(split(0)))
						word.Index = cnt.getAndIncrement()
						word.incrementSequencesCount(Convert.ToInt64(split(2)))

						vocabCache.addToken(word)
						vocabCache.addWordToIndex(word.Index, word.Label)

						Nd4j.MemoryManager.invokeGcOccasionally()
							line = reader.ReadLine()
					Loop
				End Using
			End If

			Dim rows As IList(Of INDArray) = New List(Of INDArray)()
			' basically read up everything, call vstacl and then return model
			Try
					Using reader As Reader = New CSVReader(tmpFileSyn0)
					Dim cnt As New AtomicInteger(0)
					Do While reader.hasNext()
						Dim pair As Pair(Of VocabWord, Single()) = reader.next()
						Dim word As VocabWord = pair.First
						Dim vector As INDArray = Nd4j.create(pair.Second)
        
						If ve IsNot Nothing Then
							If syn0 Is Nothing Then
								syn0 = Nd4j.create(vocabCache.numWords(), vector.length())
							End If
        
							syn0.getRow(cnt.getAndIncrement()).assign(vector)
						Else
							rows.Add(vector)
        
							vocabCache.addToken(word)
							vocabCache.addWordToIndex(word.Index, word.Label)
						End If
        
						Nd4j.MemoryManager.invokeGcOccasionally()
					Loop
					End Using
			Catch e As Exception
				Throw New Exception(e)
			Finally
				If originalPeriodic Then
					Nd4j.MemoryManager.togglePeriodicGc(True)
				End If

				Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq
				Try
					If tmpFileSyn0 IsNot Nothing Then
						tmpFileSyn0.delete()
					End If

					If tmpFileConfig IsNot Nothing Then
						tmpFileConfig.delete()
					End If
				Catch e As Exception
				End Try 'Ignore
			End Try

			If syn0 Is Nothing AndAlso vocabCache.numWords() > 0 Then
				syn0 = Nd4j.vstack(rows)
			End If

			If syn0 Is Nothing Then
				log.error("Can't build syn0 table")
				Throw New DL4JInvalidInputException("Can't build syn0 table")
			End If

			lookupTable = (New InMemoryLookupTable.Builder(Of VocabWord)()).cache(vocabCache).vectorLength(syn0.columns()).useHierarchicSoftmax(False).useAdaGrad(False).build()

			lookupTable.setSyn0(syn0)

			Dim builder As Word2Vec.Builder = (New Word2Vec.Builder(configuration)).lookupTable(lookupTable).useAdaGrad(False).vocabCache(vocabCache).layerSize(lookupTable.layerSize()).useHierarchicSoftmax(False).resetModel(False)

	'        
	'            Trying to restore TokenizerFactory & TokenPreProcessor
	'         

			Dim factory As TokenizerFactory = getTokenizerFactory(configuration)
			If factory IsNot Nothing Then
				builder.tokenizerFactory(factory)
			End If

			vec = builder.build()

			Return vec
		End Function

		Protected Friend Shared Function getTokenizerFactory(ByVal configuration As VectorsConfiguration) As TokenizerFactory
			If configuration Is Nothing Then
				Return Nothing
			End If

			Dim tokenizerFactoryClassName As String = configuration.getTokenizerFactory()
			If StringUtils.isNotEmpty(tokenizerFactoryClassName) Then
				Dim factory As TokenizerFactory = DL4JClassLoading.createNewInstance(tokenizerFactoryClassName)

				Dim tokenPreProcessorClassName As String = configuration.getTokenPreProcessor()
				If StringUtils.isNotEmpty(tokenPreProcessorClassName) Then
					Dim preProcessor As Object = DL4JClassLoading.createNewInstance(tokenizerFactoryClassName)
					If TypeOf preProcessor Is TokenPreProcess Then
						Dim tokenPreProcess As TokenPreProcess = DirectCast(preProcessor, TokenPreProcess)
						factory.TokenPreProcessor = tokenPreProcess
					Else
						log.warn("Found instance of {}, was not actually a pre processor. Ignoring.",tokenPreProcessorClassName)
					End If
				End If

				Return factory
			End If

			Return Nothing
		End Function

		''' <summary>
		''' This method restores previously saved w2v model. File can be in one of the following formats:
		''' 1) Binary model, either compressed or not. Like well-known Google Model
		''' 2) Popular CSV word2vec text format
		''' 3) DL4j compressed format
		''' 
		''' In return you get StaticWord2Vec model, which might be used as lookup table only in multi-gpu environment.
		''' </summary>
		''' <param name="inputStream"> InputStream should point to previously saved w2v model
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.models.embeddings.wordvectors.WordVectors loadStaticModel(java.io.InputStream inputStream) throws java.io.IOException
		Public Shared Function loadStaticModel(ByVal inputStream As Stream) As WordVectors

			Dim tmpFile As File = DL4JFileUtils.createTempFile("word2vec" & DateTimeHelper.CurrentUnixTimeMillis(), ".tmp")
			FileUtils.copyInputStreamToFile(inputStream, tmpFile)
			Try
				Return loadStaticModel(tmpFile)
			Finally
				tmpFile.delete()
			End Try

		End Function

		' TODO: this method needs better name :)
		''' <summary>
		''' This method restores previously saved w2v model. File can be in one of the following formats:
		''' 1) Binary model, either compressed or not. Like well-known Google Model
		''' 2) Popular CSV word2vec text format
		''' 3) DL4j compressed format
		''' 
		''' In return you get StaticWord2Vec model, which might be used as lookup table only in multi-gpu environment.
		''' </summary>
		''' <param name="file"> File
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.embeddings.wordvectors.WordVectors loadStaticModel(@NonNull File file)
		Public Shared Function loadStaticModel(ByVal file As File) As WordVectors
			If Not file.exists() OrElse file.isDirectory() Then
				Throw New Exception(New FileNotFoundException("File [" & file.getAbsolutePath() & "] was not found"))
			End If

			Dim originalFreq As Integer = Nd4j.MemoryManager.OccasionalGcFrequency
			Dim originalPeriodic As Boolean = Nd4j.MemoryManager.PeriodicGcActive

			If originalPeriodic Then
				Nd4j.MemoryManager.togglePeriodicGc(False)
			End If

			Nd4j.MemoryManager.OccasionalGcFrequency = 50000

			Dim storage As CompressedRamStorage(Of Integer) = (New CompressedRamStorage.Builder(Of Integer)()).useInplaceCompression(False).setCompressor(New NoOp()).emulateIsAbsent(False).build()

			Dim vocabCache As VocabCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()


			' now we need to define which file format we have here
			' if zip - that's dl4j format
			Try
				log.debug("Trying DL4j format...")
				Dim tmpFileSyn0 As File = DL4JFileUtils.createTempFile("word2vec", "syn")
				tmpFileSyn0.deleteOnExit()

				Dim zipFile As New ZipFile(file)
				Dim syn0 As ZipEntry = zipFile.getEntry("syn0.txt")
				Dim stream As Stream = zipFile.getInputStream(syn0)

				FileUtils.copyInputStreamToFile(stream, tmpFileSyn0)
				storage.clear()

				Try
						Using reader As Reader = New CSVReader(tmpFileSyn0)
						Do While reader.hasNext()
							Dim pair As Pair(Of VocabWord, Single()) = reader.next()
							Dim word As VocabWord = pair.First
							storage.store(word.Index, pair.Second)
        
							vocabCache.addToken(word)
							vocabCache.addWordToIndex(word.Index, word.Label)
        
							Nd4j.MemoryManager.invokeGcOccasionally()
						Loop
						End Using
				Catch e As Exception
					Throw New Exception(e)
				Finally
					If originalPeriodic Then
						Nd4j.MemoryManager.togglePeriodicGc(True)
					End If

					Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq

					Try
						tmpFileSyn0.delete()
					Catch e As Exception
						'
					End Try
				End Try
			Catch e As Exception
				'
				Try
					' try to load file as text csv
					vocabCache = (New AbstractCache.Builder(Of VocabWord)()).build()
					storage.clear()
					log.debug("Trying CSVReader...")
					Try
							Using reader As Reader = New CSVReader(file)
							Do While reader.hasNext()
								Dim pair As Pair(Of VocabWord, Single()) = reader.next()
								Dim word As VocabWord = pair.First
								storage.store(word.Index, pair.Second)
        
								vocabCache.addToken(word)
								vocabCache.addWordToIndex(word.Index, word.Label)
        
								Nd4j.MemoryManager.invokeGcOccasionally()
							Loop
							End Using
					Catch ef As Exception
						' we throw away this exception, and trying to load data as binary model
						Throw New Exception(ef)
					Finally
						If originalPeriodic Then
							Nd4j.MemoryManager.togglePeriodicGc(True)
						End If

						Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq
					End Try
				Catch ex As Exception
					' otherwise it's probably google model. which might be compressed or not
					log.debug("Trying BinaryReader...")
					vocabCache = (New AbstractCache.Builder(Of VocabWord)()).build()
					storage.clear()
					Try
							Using reader As Reader = New BinaryReader(file)
							Do While reader.hasNext()
								Dim pair As Pair(Of VocabWord, Single()) = reader.next()
								Dim word As VocabWord = pair.First
        
								storage.store(word.Index, pair.Second)
        
								vocabCache.addToken(word)
								vocabCache.addWordToIndex(word.Index, word.Label)
        
								Nd4j.MemoryManager.invokeGcOccasionally()
							Loop
							End Using
					Catch ez As Exception
						Throw New Exception("Unable to guess input file format")
					Finally
						If originalPeriodic Then
							Nd4j.MemoryManager.togglePeriodicGc(True)
						End If

						Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq
					End Try
				Finally
					If originalPeriodic Then
						Nd4j.MemoryManager.togglePeriodicGc(True)
					End If

					Nd4j.MemoryManager.OccasionalGcFrequency = originalFreq
				End Try
			End Try


			Dim word2Vec As StaticWord2Vec = (New StaticWord2Vec.Builder(storage, vocabCache)).build()

			Return word2Vec
		End Function


		Protected Friend Interface Reader
			Inherits AutoCloseable

			Function hasNext() As Boolean

			Function [next]() As Pair(Of VocabWord, Single())
		End Interface


		Protected Friend Class BinaryReader
			Implements Reader

			Protected Friend stream As DataInputStream
			Protected Friend nextWord As String
			Protected Friend numWords As Integer
			Protected Friend vectorLength As Integer
			Protected Friend idxCounter As New AtomicInteger(0)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BinaryReader(@NonNull File file)
			Protected Friend Sub New(ByVal file As File)
				Try
					' Try to read as GZip
					stream = New DataInputStream(New BufferedInputStream(New GZIPInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))))
				Catch e As IOException
					Try
						' Failed to read as Gzip, assuming it's not compressed binary format
						stream = New DataInputStream(New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read)))
					Catch e1 As Exception
						Throw New Exception(e1)
					End Try
				Catch e As Exception
					Throw New Exception(e)
				End Try
				Try
					numWords = Integer.Parse(ReadHelper.readString(stream))
					vectorLength = Integer.Parse(ReadHelper.readString(stream))
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End Sub

			Public Overridable Function hasNext() As Boolean Implements Reader.hasNext
				Return idxCounter.get() < numWords
			End Function

			Public Overridable Function [next]() As Pair(Of VocabWord, Single()) Implements Reader.next
				Try
					Dim word As String = ReadHelper.readString(stream)
					Dim element As New VocabWord(1.0, word)
					element.Index = idxCounter.getAndIncrement()

					Dim vector(vectorLength - 1) As Single
					For i As Integer = 0 To vectorLength - 1
						vector(i) = ReadHelper.readFloat(stream)
					Next i

					Return Pair.makePair(element, vector)
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
			Public Overrides Sub close()
				If stream IsNot Nothing Then
					stream.close()
				End If
			End Sub
		End Class

		Protected Friend Class CSVReader
			Implements Reader

			Friend reader As StreamReader
			Friend idxCounter As New AtomicInteger(0)
			Friend nextLine As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected CSVReader(@NonNull File file)
			Protected Friend Sub New(ByVal file As File)
				Try
					reader = New StreamReader(file)
					nextLine = reader.ReadLine()

					' checking if there's header inside
					Dim split() As String = nextLine.Split(" ", True)
					Try
						If Integer.Parse(split(0)) > 0 AndAlso split.Length <= 5 Then
							' this is header. skip it.
							nextLine = reader.ReadLine()
						End If
					Catch e As Exception
						' this is proper string, do nothing
					End Try
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Sub

			Public Overridable Function hasNext() As Boolean Implements Reader.hasNext
				Return nextLine IsNot Nothing
			End Function

			Public Overridable Function [next]() As Pair(Of VocabWord, Single()) Implements Reader.next

				Dim split() As String = nextLine.Split(" ", True)

				Dim word As New VocabWord(1.0, ReadHelper.decodeB64(split(0)))
				word.Index = idxCounter.getAndIncrement()

				Dim vector(split.Length - 2) As Single
				For i As Integer = 1 To split.Length - 1
					vector(i - 1) = Single.Parse(split(i))
				Next i

				Try
					nextLine = reader.ReadLine()
				Catch e As Exception
					nextLine = Nothing
				End Try

				Return Pair.makePair(word, vector)
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
			Public Overrides Sub close()
				If reader IsNot Nothing Then
					reader.Close()
				End If
			End Sub
		End Class

		''' <summary>
		''' This method saves Word2Vec model to output stream
		''' </summary>
		''' <param name="word2Vec"> Word2Vec </param>
		''' <param name="stream"> OutputStream </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeWord2Vec(@NonNull Word2Vec word2Vec, @NonNull OutputStream stream) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeWord2Vec(ByVal word2Vec As Word2Vec, ByVal stream As Stream)

			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(word2Vec.getConfiguration())).layerSize(word2Vec.getLayerSize()).build()
			vectors.Vocab = word2Vec.getVocab()
			vectors.LookupTable = word2Vec.getLookupTable()
			vectors.ModelUtils = word2Vec.getModelUtils()
			writeSequenceVectors(vectors, stream)
		End Sub

		''' <summary>
		''' This method restores Word2Vec model from file
		''' </summary>
		''' <param name="path"> </param>
		''' <param name="readExtendedTables"> </param>
		''' <returns> Word2Vec </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readWord2Vec(@NonNull String path, boolean readExtendedTables)
		Public Shared Function readWord2Vec(ByVal path As String, ByVal readExtendedTables As Boolean) As Word2Vec
			Dim file As New File(path)
			Return readWord2Vec(file, readExtendedTables)
		End Function

		''' <summary>
		''' This method saves table of weights to file
		''' </summary>
		''' <param name="weightLookupTable"> WeightLookupTable </param>
		''' <param name="file"> File </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> void writeLookupTable(org.deeplearning4j.models.embeddings.WeightLookupTable<T> weightLookupTable, @NonNull File file) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeLookupTable(Of T As SequenceElement)(ByVal weightLookupTable As WeightLookupTable(Of T), ByVal file As File)
			Using writer As New StreamWriter(New FileStream(file, FileMode.Create, FileAccess.Write), Encoding.UTF8)
				Dim numWords As Integer = weightLookupTable.getVocabCache().numWords()
				Dim layersSize As Integer = weightLookupTable.layerSize()
				Dim totalNumberOfDocs As Long = weightLookupTable.getVocabCache().totalNumberOfDocs()

				Dim format As String = "%d %d %d" & vbLf
				Dim header As String = String.format(format, numWords, layersSize, totalNumberOfDocs)

				writer.Write(header)

				Dim row As String = ""
				Dim j As Integer = 0
				Do While j < weightLookupTable.getVocabCache().words().Count
					Dim label As String = weightLookupTable.getVocabCache().wordAtIndex(j)
					row &= label & " "
					Dim freq As Integer = weightLookupTable.getVocabCache().wordFrequency(label)
					Dim rows As Integer = CType(weightLookupTable, InMemoryLookupTable).getSyn0().rows()
					Dim cols As Integer = CType(weightLookupTable, InMemoryLookupTable).getSyn0().columns()
					row &= freq & " " & rows & " " & cols & " "

					For r As Integer = 0 To rows - 1
						'row += " ";
						For c As Integer = 0 To cols - 1
							row &= CType(weightLookupTable, InMemoryLookupTable).getSyn0().getDouble(r, c) & " "
						Next c
						'row += " ";
					Next r
					row &= vbLf
					j += 1
				Loop
				writer.Write(row)
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.embeddings.WeightLookupTable<T> readLookupTable(java.io.File file) throws java.io.IOException
		Public Shared Function readLookupTable(Of T As SequenceElement)(ByVal file As File) As WeightLookupTable(Of T)
			Return readLookupTable(New FileStream(file, FileMode.Open, FileAccess.Read))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> org.deeplearning4j.models.embeddings.WeightLookupTable<T> readLookupTable(java.io.InputStream stream) throws java.io.IOException
		Public Shared Function readLookupTable(Of T As SequenceElement)(ByVal stream As Stream) As WeightLookupTable(Of T)
			Dim weightLookupTable As WeightLookupTable(Of T) = Nothing
			Dim vocabCache As New AbstractCache(Of VocabWord)()
			Const startSyn0 As Integer = 4
			Dim headerRead As Boolean = False
			Dim numWords As Integer = -1, layerSize As Integer = -1, totalNumberOfDocs As Integer = -1
			Try
				Dim syn0 As INDArray = Nothing
				Dim index As Integer = 0
				For Each line As String In IOUtils.readLines(stream)
					Dim tokens() As String = line.Split(" ", True)
					If Not headerRead Then
						' reading header as "NUM_WORDS VECTOR_SIZE NUM_DOCS"
						numWords = Integer.Parse(tokens(0))
						layerSize = Integer.Parse(tokens(1))
						totalNumberOfDocs = Integer.Parse(tokens(2))
						log.debug("Reading header - words: {}, layerSize: {}, totalNumberOfDocs: {}", numWords, layerSize, totalNumberOfDocs)
						headerRead = True
						weightLookupTable = (New InMemoryLookupTable.Builder()).cache(vocabCache).vectorLength(layerSize).build()
					Else
						Dim label As String = ReadHelper.decodeB64(tokens(0))
						Dim freq As Integer = Integer.Parse(tokens(1))
						Dim rows As Integer = Integer.Parse(tokens(2))
						Dim cols As Integer = Integer.Parse(tokens(3))

						If syn0 Is Nothing Then
							syn0 = Nd4j.createUninitialized(rows, cols)
						End If

						Dim i As Integer = startSyn0
						For r As Integer = 0 To rows - 1
							Dim vector(cols - 1) As Double
							For c As Integer = 0 To cols - 1
								vector(c) = Double.Parse(tokens(i))
								i += 1
							Next c
							syn0.putRow(r, Nd4j.create(vector))
						Next r

						Dim vw As New VocabWord(freq, label)
						vw.Index = index
						weightLookupTable.getVocabCache().addToken(CType(vw, T))
						weightLookupTable.getVocabCache().addWordToIndex(index, label)
						index += 1
					End If
				Next line
				CType(weightLookupTable, InMemoryLookupTable(Of T)).setSyn0(syn0)
			Finally
				stream.Close()
			End Try
			Return weightLookupTable
		End Function

		''' <summary>
		''' This method loads Word2Vec model from file
		''' </summary>
		''' <param name="file"> File </param>
		''' <param name="readExtendedTables"> boolean </param>
		''' <returns> Word2Vec </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readWord2Vec(@NonNull File file, boolean readExtendedTables)
		Public Shared Function readWord2Vec(ByVal file As File, ByVal readExtendedTables As Boolean) As Word2Vec
			Try
					Using inputStream As Stream = fileStream(file)
					Return readWord2Vec(inputStream, readExtendedTables)
					End Using
			Catch readSequenceVectors As Exception
				Throw New Exception(readSequenceVectors)
			End Try
		End Function

		''' <summary>
		''' This method loads Word2Vec model from input stream
		''' </summary>
		''' <param name="stream"> InputStream </param>
		''' <param name="readExtendedTable"> boolean </param>
		''' <returns> Word2Vec </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.models.word2vec.Word2Vec readWord2Vec(@NonNull InputStream stream, boolean readExtendedTable) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function readWord2Vec(ByVal stream As Stream, ByVal readExtendedTable As Boolean) As Word2Vec
			Dim vectors As SequenceVectors(Of VocabWord) = readSequenceVectors(stream, readExtendedTable)

			Dim word2Vec As Word2Vec = (New Word2Vec.Builder(vectors.getConfiguration())).layerSize(vectors.LayerSize).build()
			word2Vec.Vocab = vectors.getVocab()
			word2Vec.LookupTable = vectors.lookupTable()
			word2Vec.ModelUtils = vectors.getModelUtils()

			Return word2Vec
		End Function

		''' <summary>
		''' This method loads FastText model to file
		''' </summary>
		''' <param name="vectors"> FastText </param>
		''' <param name="path"> File </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeWordVectors(@NonNull FastText vectors, @NonNull File path) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeWordVectors(ByVal vectors As FastText, ByVal path As File)
			Dim outputStream As ObjectOutputStream = Nothing
			Try
				outputStream = New ObjectOutputStream(New FileStream(path, FileMode.Create, FileAccess.Write))
				outputStream.writeObject(vectors)
			Finally
				Try
					If outputStream IsNot Nothing Then
						outputStream.flush()
						outputStream.close()
					End If
				Catch ex As IOException
					Console.WriteLine(ex.ToString())
					Console.Write(ex.StackTrace)
				End Try
			End Try
		End Sub

		''' <summary>
		''' This method unloads FastText model from file
		''' </summary>
		''' <param name="path"> File </param>
		Public Shared Function readWordVectors(ByVal path As File) As FastText
			Dim result As FastText = Nothing
			Try
				Dim fileIn As New FileStream(path, FileMode.Open, FileAccess.Read)
				Dim [in] As New ObjectInputStream(fileIn)
				Try
					result = CType([in].readObject(), FastText)
				Catch ex As ClassNotFoundException

				End Try
			Catch ex As FileNotFoundException
				Console.WriteLine(ex.ToString())
				Console.Write(ex.StackTrace)
			Catch ex As IOException
				Console.WriteLine(ex.ToString())
				Console.Write(ex.StackTrace)
			End Try
			Return result
		End Function

		''' <summary>
		''' This method prints memory usage to log
		''' </summary>
		''' <param name="numWords"> </param>
		''' <param name="vectorLength"> </param>
		''' <param name="numTables"> </param>
		Public Shared Sub printOutProjectedMemoryUse(ByVal numWords As Long, ByVal vectorLength As Integer, ByVal numTables As Integer)
			Dim memSize As Double = numWords * vectorLength * Nd4j.sizeOfDataType() * numTables

			Dim sfx As String
			Dim value As Double
			If memSize < 1024 * 1024L Then
				sfx = "KB"
				value = memSize / 1024
			End If
			If memSize < 1024 * 1024L * 1024L Then
				sfx = "MB"
				value = memSize / 1024 / 1024
			Else
				sfx = "GB"
				value = memSize / 1024 / 1024 / 1024
			End If

			OneTimeLogger.info(log, "Projected memory use for model: [{} {}]", String.Format("{0:F2}", value), sfx)

		End Sub

		''' <summary>
		'''   Helper static methods to read data from input stream.
		''' </summary>
		Public Class ReadHelper
			''' <summary>
			''' Read a float from a data input stream Credit to:
			''' https://github.com/NLPchina/Word2VEC_java/blob/master/src/com/ansj/vec/Word2VEC.java
			''' </summary>
			''' <param name="is">
			''' @return </param>
			''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static float readFloat(java.io.InputStream is) throws java.io.IOException
			Friend Shared Function readFloat(ByVal [is] As Stream) As Single
				Dim bytes(3) As SByte
				[is].Read(bytes, 0, bytes.Length)
				Return getFloat(bytes)
			End Function

			''' <summary>
			''' Read a string from a data input stream Credit to:
			''' https://github.com/NLPchina/Word2VEC_java/blob/master/src/com/ansj/vec/Word2VEC.java
			''' </summary>
			''' <param name="b">
			''' @return </param>
			''' <exception cref="IOException"> </exception>
			Friend Shared Function getFloat(ByVal b() As SByte) As Single
				Dim accum As Integer = 0
				accum = accum Or (b(0) And &Hff) << 0
				accum = accum Or (b(1) And &Hff) << 8
				accum = accum Or (b(2) And &Hff) << 16
				accum = accum Or (b(3) And &Hff) << 24
				Return Float.intBitsToFloat(accum)
			End Function

			''' <summary>
			''' Read a string from a data input stream Credit to:
			''' https://github.com/NLPchina/Word2VEC_java/blob/master/src/com/ansj/vec/Word2VEC.java
			''' </summary>
			''' <param name="dis">
			''' @return </param>
			''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static String readString(java.io.DataInputStream dis) throws java.io.IOException
			Friend Shared Function readString(ByVal dis As DataInputStream) As String
				Dim bytes(MAX_SIZE - 1) As SByte
				Dim b As SByte = dis.readByte()
				Dim i As Integer = -1
				Dim sb As New StringBuilder()
				Do While b <> 32 AndAlso b <> 10
					i += 1
					bytes(i) = b
					b = dis.readByte()
					If i = 49 Then
						sb.Append(StringHelper.NewString(bytes, "UTF-8"))
						i = -1
						bytes = New SByte(MAX_SIZE - 1){}
					End If
				Loop
				sb.Append(StringHelper.NewString(bytes, 0, i + 1, "UTF-8"))
				Return sb.ToString()
			End Function

			Friend Const B64 As String = "B64:"

			''' <summary>
			''' Encode input string
			''' </summary>
			''' <param name="word"> String </param>
			''' <returns> String </returns>
			Public Shared Function encodeB64(ByVal word As String) As String
				Try
					Return B64 + Base64.encodeBase64String(word.GetBytes(Encoding.UTF8)).replaceAll("(" & vbCr & "|" & vbLf & ")", "")
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Function

			''' <summary>
			''' Encode input string
			''' </summary>
			''' <param name="word"> String </param>
			''' <returns> String </returns>

			Public Shared Function decodeB64(ByVal word As String) As String
				If word.StartsWith(B64, StringComparison.Ordinal) Then
					Dim arp As String = word.replaceFirst(B64, "")
					Try
						Return New String(Base64.decodeBase64(arp), "UTF-8")
					Catch e As Exception
						Throw New Exception(e)
					End Try
				Else
					Return word
				End If
			End Function
		End Class
	End Class

End Namespace