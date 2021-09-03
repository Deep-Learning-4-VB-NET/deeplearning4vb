Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.integration.testcases.dl4j.misc


	<Serializable>
	Public Class CharacterIterator
		Implements DataSetIterator

		'Valid characters
		Private validCharacters() As Char
		'Maps each character to an index ind the input/output
		Private charToIdxMap As IDictionary(Of Char, Integer)
		'All characters of the input file (after filtering to only those that are valid
		Private fileCharacters() As Char
		'Length of each example/minibatch (number of characters)
		Private exampleLength As Integer
		'Size of each minibatch (number of examples)
		Private miniBatchSize As Integer
		Private rng As Random
		'Offsets for the start of each example
		Private exampleStartOffsets As New LinkedList(Of Integer)()

		''' <param name="textFilePath">     Path to text file to use for generating samples </param>
		''' <param name="textFileEncoding"> Encoding of the text file. Can try Charset.defaultCharset() </param>
		''' <param name="miniBatchSize">    Number of examples per mini-batch </param>
		''' <param name="exampleLength">    Number of characters in each input/output vector </param>
		''' <param name="validCharacters">  Character array of valid characters. Characters not present in this array will be removed </param>
		''' <param name="rng">              Random number generator, for repeatability if required </param>
		''' <exception cref="IOException"> If text file cannot  be loaded </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public CharacterIterator(String textFilePath, java.nio.charset.Charset textFileEncoding, int miniBatchSize, int exampleLength, char[] validCharacters, Random rng) throws java.io.IOException
		Public Sub New(ByVal textFilePath As String, ByVal textFileEncoding As Charset, ByVal miniBatchSize As Integer, ByVal exampleLength As Integer, ByVal validCharacters() As Char, ByVal rng As Random)
			Me.New(textFilePath, textFileEncoding, miniBatchSize, exampleLength, validCharacters, rng, Nothing)
		End Sub

		''' <param name="textFilePath">     Path to text file to use for generating samples </param>
		''' <param name="textFileEncoding"> Encoding of the text file. Can try Charset.defaultCharset() </param>
		''' <param name="miniBatchSize">    Number of examples per mini-batch </param>
		''' <param name="exampleLength">    Number of characters in each input/output vector </param>
		''' <param name="validCharacters">  Character array of valid characters. Characters not present in this array will be removed </param>
		''' <param name="rng">              Random number generator, for repeatability if required </param>
		''' <param name="commentChars">     if non-null, lines starting with this string are skipped. </param>
		''' <exception cref="IOException"> If text file cannot  be loaded </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public CharacterIterator(String textFilePath, java.nio.charset.Charset textFileEncoding, int miniBatchSize, int exampleLength, char[] validCharacters, Random rng, String commentChars) throws java.io.IOException
		Public Sub New(ByVal textFilePath As String, ByVal textFileEncoding As Charset, ByVal miniBatchSize As Integer, ByVal exampleLength As Integer, ByVal validCharacters() As Char, ByVal rng As Random, ByVal commentChars As String)
			If Not Directory.Exists(textFilePath) OrElse File.Exists(textFilePath) Then
				Throw New IOException("Could not access file (does not exist): " & textFilePath)
			End If
			If miniBatchSize <= 0 Then
				Throw New System.ArgumentException("Invalid miniBatchSize (must be >0)")
			End If
			Me.validCharacters = validCharacters
			Me.exampleLength = exampleLength
			Me.miniBatchSize = miniBatchSize
			Me.rng = rng

			'Store valid characters is a map for later use in vectorization
			charToIdxMap = New Dictionary(Of Char, Integer)()
			For i As Integer = 0 To validCharacters.Length - 1
				charToIdxMap(validCharacters(i)) = i
			Next i

			'Load file and convert contents to a char[]
			Dim newLineValid As Boolean = charToIdxMap.ContainsKey(ControlChars.Lf)
			Dim lines As IList(Of String) = Files.readAllLines((New File(textFilePath)).toPath(), textFileEncoding)
			If commentChars IsNot Nothing Then
				Dim withoutComments As IList(Of String) = New List(Of String)()
				For Each line As String In lines
					If Not line.StartsWith(commentChars, StringComparison.Ordinal) Then
						withoutComments.Add(line)
					End If
				Next line
				lines = withoutComments
			End If
			Dim maxSize As Integer = lines.Count 'add lines.size() to account for newline characters at end of each line
			For Each s As String In lines
				maxSize += s.Length
			Next s
			Dim characters(maxSize - 1) As Char
			Dim currIdx As Integer = 0
			For Each s As String In lines
				Dim thisLine() As Char = s.ToCharArray()
				For Each aThisLine As Char In thisLine
					If Not charToIdxMap.ContainsKey(aThisLine) Then
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: characters[currIdx++] = aThisLine;
					characters(currIdx) = aThisLine
						currIdx += 1
				Next aThisLine
				If newLineValid Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: characters[currIdx++] = ControlChars.Lf;
					characters(currIdx) = ControlChars.Lf
						currIdx += 1
				End If
			Next s

			If currIdx = characters.Length Then
				fileCharacters = characters
			Else
				fileCharacters = Arrays.CopyOfRange(characters, 0, currIdx)
			End If
			If exampleLength >= fileCharacters.Length Then
				Throw New System.ArgumentException("exampleLength=" & exampleLength & " cannot exceed number of valid characters in file (" & fileCharacters.Length & ")")
			End If

	'        int nRemoved = maxSize - fileCharacters.length;
	'        System.out.println("Loaded and converted file: " + fileCharacters.length + " valid characters of "
	'                + maxSize + " total characters (" + nRemoved + " removed)");

			initializeOffsets()
		End Sub

		''' <summary>
		''' A minimal character set, with a-z, A-Z, 0-9 and common punctuation etc
		''' </summary>
		Public Shared ReadOnly Property MinimalCharacterSet As Char()
			Get
				Dim validChars As IList(Of Char) = New LinkedList(Of Char)()
				For c As Char = AscW("a"c) To AscW("z"c)
					validChars.Add(c)
				Next c
				For c As Char = AscW("A"c) To AscW("Z"c)
					validChars.Add(c)
				Next c
				For c As Char = AscW("0"c) To AscW("9"c)
					validChars.Add(c)
				Next c
				Dim temp() As Char = {"!"c, "&"c, "("c, ")"c, "?"c, "-"c, "'"c, """"c, ","c, "."c, ":"c, ";"c, " "c, ControlChars.Lf, ControlChars.Tab}
				For Each c As Char In temp
					validChars.Add(c)
				Next c
				Dim [out](validChars.Count - 1) As Char
				Dim i As Integer = 0
				For Each c As Char? In validChars
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: out[i++] = c;
					[out](i) = c
						i += 1
				Next c
				Return [out]
			End Get
		End Property

		''' <summary>
		''' As per getMinimalCharacterSet(), but with a few extra characters
		''' </summary>
		Public Shared ReadOnly Property DefaultCharacterSet As Char()
			Get
				Dim validChars As IList(Of Char) = New LinkedList(Of Char)()
				For Each c As Char In MinimalCharacterSet
					validChars.Add(c)
				Next c
				Dim additionalChars() As Char = {"@"c, "#"c, "$"c, "%"c, "^"c, "*"c, "{"c, "}"c, "["c, "]"c, "/"c, "+"c, "_"c, "\"c, "|"c, "<"c, ">"c}
				For Each c As Char In additionalChars
					validChars.Add(c)
				Next c
				Dim [out](validChars.Count - 1) As Char
				Dim i As Integer = 0
				For Each c As Char? In validChars
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: out[i++] = c;
					[out](i) = c
						i += 1
				Next c
				Return [out]
			End Get
		End Property

		Public Overridable Function convertIndexToCharacter(ByVal idx As Integer) As Char
			Return validCharacters(idx)
		End Function

		Public Overridable Function convertCharacterToIndex(ByVal c As Char) As Integer
			Return charToIdxMap(c)
		End Function

		Public Overridable ReadOnly Property RandomCharacter As Char
			Get
				Return validCharacters(CInt(rng.NextDouble() * validCharacters.Length))
			End Get
		End Property

		Public Overridable Function hasNext() As Boolean
			Return exampleStartOffsets.Count > 0
		End Function

		Public Overridable Function [next]() As DataSet
			Return [next](miniBatchSize)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			If exampleStartOffsets.Count = 0 Then
				Throw New NoSuchElementException()
			End If

			Dim currMinibatchSize As Integer = Math.Min(num, exampleStartOffsets.Count)
			'Allocate space:
			'Note the order here:
			' dimension 0 = number of examples in minibatch
			' dimension 1 = size of each vector (i.e., number of characters)
			' dimension 2 = length of each time series/example
			'Why 'f' order here? See https://deeplearning4j.konduit.ai/models/recurrent data section "Alternative: Implementing a custom DataSetIterator"
			Dim input As INDArray = Nd4j.create(New Integer(){currMinibatchSize, validCharacters.Length, exampleLength}, "f"c)
			Dim labels As INDArray = Nd4j.create(New Integer(){currMinibatchSize, validCharacters.Length, exampleLength}, "f"c)

			For i As Integer = 0 To currMinibatchSize - 1
				Dim startIdx As Integer = exampleStartOffsets.RemoveFirst()
				Dim endIdx As Integer = startIdx + exampleLength
				Dim currCharIdx As Integer = charToIdxMap(fileCharacters(startIdx)) 'Current input
				Dim c As Integer = 0
				Dim j As Integer = startIdx + 1
				Do While j < endIdx
					Dim nextCharIdx As Integer = charToIdxMap(fileCharacters(j)) 'Next character to predict
					input.putScalar(New Integer(){i, currCharIdx, c}, 1.0)
					labels.putScalar(New Integer(){i, nextCharIdx, c}, 1.0)
					currCharIdx = nextCharIdx
					j += 1
					c += 1
				Loop
			Next i

			Return New DataSet(input, labels)
		End Function

		Public Overridable Function totalExamples() As Integer
			Return (fileCharacters.Length - 1) \ miniBatchSize - 2
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return validCharacters.Length
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return validCharacters.Length
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			exampleStartOffsets.Clear()
			initializeOffsets()
		End Sub

		Private Sub initializeOffsets()
			'This defines the order in which parts of the file are fetched
			Dim nMinibatchesPerEpoch As Integer = (fileCharacters.Length - 1) \ exampleLength - 2 '-2: for end index, and for partial example
			For i As Integer = 0 To nMinibatchesPerEpoch - 1
				exampleStartOffsets.AddLast(i * exampleLength)
			Next i
			Collections.shuffle(exampleStartOffsets, rng)
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return miniBatchSize
		End Function

		Public Overridable Function cursor() As Integer
			Return totalExamples() - exampleStartOffsets.Count
		End Function

		Public Overridable Function numExamples() As Integer
			Return totalExamples()
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Throw New System.NotSupportedException("Not implemented")
			End Set
			Get
				Throw New System.NotSupportedException("Not implemented")
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Throw New System.NotSupportedException("Not implemented")
			End Get
		End Property

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub


		''' <summary>
		''' Downloads Shakespeare training data and stores it locally (temp directory). Then set up and return a simple
		''' DataSetIterator that does vectorization based on the text.
		''' </summary>
		''' <param name="miniBatchSize">  Number of text segments in each training mini-batch </param>
		''' <param name="sequenceLength"> Number of characters in each text segment. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static CharacterIterator getShakespeareIterator(int miniBatchSize, int sequenceLength) throws Exception
		Public Shared Function getShakespeareIterator(ByVal miniBatchSize As Integer, ByVal sequenceLength As Integer) As CharacterIterator
			'The Complete Works of William Shakespeare
			'5.3MB file in UTF-8 Encoding, ~5.4 million characters
			'https://www.gutenberg.org/ebooks/100
			Dim url As String = "https://s3.amazonaws.com/dl4j-distribution/pg100.txt"
			Dim tempDir As String = System.getProperty("java.io.tmpdir")
			Dim fileLocation As String = tempDir & "/Shakespeare.txt" 'Storage location from downloaded file
			Dim f As New File(fileLocation)
			If Not f.exists() Then
				FileUtils.copyURLToFile(New URL(url), f)
	'            System.out.println("File downloaded to " + f.getAbsolutePath());
			Else
	'            System.out.println("Using existing text file at " + f.getAbsolutePath());
			End If

			If Not f.exists() Then
				Throw New IOException("File does not exist: " & fileLocation) 'Download problem?
			End If

			Dim validCharacters() As Char = CharacterIterator.MinimalCharacterSet 'Which characters are allowed? Others will be removed
			Return New CharacterIterator(fileLocation, Charset.forName("UTF-8"), miniBatchSize, sequenceLength, validCharacters, New Random(12345))
		End Function

	End Class

End Namespace