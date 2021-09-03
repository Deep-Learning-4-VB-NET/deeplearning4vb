Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Value = lombok.Value
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports org.nd4j.linalg.dataset.api.preprocessor

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.serializer


	Public Class NormalizerSerializer

		Private Const HEADER As String = "NORMALIZER"
		Private Shared defaultSerializer As NormalizerSerializer

		Private strategies As IList(Of NormalizerSerializerStrategy) = New List(Of NormalizerSerializerStrategy)()

		''' <summary>
		''' Serialize a normalizer to the given file
		''' </summary>
		''' <param name="normalizer"> the normalizer </param>
		''' <param name="file">       the destination file </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void write(@NonNull Normalizer normalizer, @NonNull File file) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub write(ByVal normalizer As Normalizer, ByVal file As File)
			Using [out] As Stream = New BufferedOutputStream(New FileStream(file, FileMode.Create, FileAccess.Write))
				write(normalizer, [out])
			End Using
		End Sub

		''' <summary>
		''' Serialize a normalizer to the given file path
		''' </summary>
		''' <param name="normalizer"> the normalizer </param>
		''' <param name="path">       the destination file path </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void write(@NonNull Normalizer normalizer, @NonNull String path) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub write(ByVal normalizer As Normalizer, ByVal path As String)
			Using [out] As Stream = New BufferedOutputStream(New FileStream(path, FileMode.Create, FileAccess.Write))
				write(normalizer, [out])
			End Using
		End Sub

		''' <summary>
		''' Serialize a normalizer to an output stream
		''' </summary>
		''' <param name="normalizer"> the normalizer </param>
		''' <param name="stream">     the output stream to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void write(@NonNull Normalizer normalizer, @NonNull OutputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub write(ByVal normalizer As Normalizer, ByVal stream As Stream)
			Dim strategy As NormalizerSerializerStrategy = getStrategy(normalizer)

			writeHeader(stream, Header.fromStrategy(strategy))
			'noinspection unchecked
			strategy.write(normalizer, stream)
		End Sub

		''' <summary>
		''' Restore a normalizer from the given path
		''' </summary>
		''' <param name="path"> path of the file containing a serialized normalizer </param>
		''' <returns> the restored normalizer </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.linalg.dataset.api.preprocessor.Normalizer> T restore(@NonNull String path) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function restore(Of T As Normalizer)(ByVal path As String) As T
			Using [in] As Stream = New BufferedInputStream(New FileStream(path, FileMode.Open, FileAccess.Read))
				Return restore([in])
			End Using
		End Function

		''' <summary>
		''' Restore a normalizer from the given file
		''' </summary>
		''' <param name="file"> the file containing a serialized normalizer </param>
		''' <returns> the restored normalizer </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.linalg.dataset.api.preprocessor.Normalizer> T restore(@NonNull File file) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function restore(Of T As Normalizer)(ByVal file As File) As T
			Using [in] As Stream = New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
				Return restore([in])
			End Using
		End Function

		''' <summary>
		''' Restore a normalizer from an input stream
		''' </summary>
		''' <param name="stream"> a stream of serialized normalizer data </param>
		''' <returns> the restored normalizer </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.linalg.dataset.api.preprocessor.Normalizer> T restore(@NonNull InputStream stream) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function restore(Of T As Normalizer)(ByVal stream As Stream) As T
'JAVA TO VB CONVERTER NOTE: The variable header was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim header_Conflict As Header = parseHeader(stream)

			'noinspection unchecked
			Return CType(getStrategy(header_Conflict).restore(stream), T)
		End Function

		''' <summary>
		''' Get the default serializer configured with strategies for the built-in normalizer implementations
		''' </summary>
		''' <returns> the default serializer </returns>
		Public Shared ReadOnly Property Default As NormalizerSerializer
			Get
				If defaultSerializer Is Nothing Then
					defaultSerializer = (New NormalizerSerializer()).addStrategy(New StandardizeSerializerStrategy()).addStrategy(New MinMaxSerializerStrategy()).addStrategy(New MultiStandardizeSerializerStrategy()).addStrategy(New MultiMinMaxSerializerStrategy()).addStrategy(New ImagePreProcessingSerializerStrategy()).addStrategy(New MultiHybridSerializerStrategy())
				End If
				Return defaultSerializer
			End Get
		End Property

		''' <summary>
		''' Add a normalizer serializer strategy
		''' </summary>
		''' <param name="strategy"> the new strategy </param>
		''' <returns> self </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NormalizerSerializer addStrategy(@NonNull NormalizerSerializerStrategy strategy)
		Public Overridable Function addStrategy(ByVal strategy As NormalizerSerializerStrategy) As NormalizerSerializer
			strategies.Add(strategy)
			Return Me
		End Function

		''' <summary>
		''' Get a serializer strategy the given normalizer
		''' </summary>
		''' <param name="normalizer"> the normalizer to find a compatible serializer strategy for </param>
		''' <returns> the compatible strategy </returns>
		Private Function getStrategy(ByVal normalizer As Normalizer) As NormalizerSerializerStrategy
			For Each strategy As NormalizerSerializerStrategy In strategies
				If strategySupportsNormalizer(strategy, normalizer.getType(), normalizer.GetType()) Then
					Return strategy
				End If
			Next strategy
			Throw New Exception(String.Format("No serializer strategy found for normalizer of class {0}. If this is a custom normalizer, you probably " & "forgot to register a corresponding custom serializer strategy with this serializer.", normalizer.GetType()))
		End Function

		''' <summary>
		''' Get a serializer strategy the given serialized file header
		''' </summary>
		''' <param name="header"> the header to find the associated serializer strategy for </param>
		''' <returns> the compatible strategy </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private NormalizerSerializerStrategy getStrategy(Header header) throws Exception
'JAVA TO VB CONVERTER NOTE: The parameter header was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Function getStrategy(ByVal header_Conflict As Header) As NormalizerSerializerStrategy
			If header_Conflict.normalizerType.Equals(NormalizerType.CUSTOM) Then
				Return System.Activator.CreateInstance(header_Conflict.customStrategyClass)
			End If
			For Each strategy As NormalizerSerializerStrategy In strategies
				If strategySupportsNormalizer(strategy, header_Conflict.normalizerType, Nothing) Then
					Return strategy
				End If
			Next strategy
			Throw New Exception("No serializer strategy found for given header " & header_Conflict)
		End Function

		''' <summary>
		''' Check if a serializer strategy supports a normalizer. If the normalizer is a custom opType, it checks if the
		''' supported normalizer class matches.
		''' </summary>
		''' <param name="strategy"> </param>
		''' <param name="normalizerType"> </param>
		''' <param name="normalizerClass"> </param>
		''' <returns> whether the strategy supports the normalizer </returns>
		Private Function strategySupportsNormalizer(ByVal strategy As NormalizerSerializerStrategy, ByVal normalizerType As NormalizerType, ByVal normalizerClass As Type) As Boolean
			If Not strategy.getSupportedType().Equals(normalizerType) Then
				Return False
			End If
			If strategy.getSupportedType().Equals(NormalizerType.CUSTOM) Then
				' Strategy should be instance of CustomSerializerStrategy
				If Not (TypeOf strategy Is CustomSerializerStrategy) Then
					Throw New System.ArgumentException("Strategies supporting CUSTOM opType must be instance of CustomSerializerStrategy, got" & strategy.GetType())
				End If
				Return CType(strategy, CustomSerializerStrategy).getSupportedClass().Equals(normalizerClass)
			End If
			Return True
		End Function

		''' <summary>
		''' Parse the data header
		''' </summary>
		''' <param name="stream"> the input stream </param>
		''' <returns> the parsed header with information about the contents </returns>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="IllegalArgumentException"> if the data format is invalid </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private Header parseHeader(InputStream stream) throws IOException
		Private Function parseHeader(ByVal stream As Stream) As Header
			Dim dis As New DataInputStream(stream)
			' Check if the stream starts with the expected header
'JAVA TO VB CONVERTER NOTE: The variable header was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim header_Conflict As String = dis.readUTF()
			If Not header_Conflict.Equals(HEADER) Then
				Throw New System.ArgumentException("Could not restore normalizer: invalid header. If this normalizer was saved with a opType-specific " & "strategy like StandardizeSerializerStrategy, use that class to restore it as well.")
			End If

			' The next byte is an integer indicating the version
			Dim version As Integer = dis.readInt()

			' Right now, we only support version 1
			If version <> 1 Then
				Throw New System.ArgumentException("Could not restore normalizer: invalid version (" & version & ")")
			End If
			' The next value is a string indicating the normalizer opType
			Dim type As NormalizerType = System.Enum.Parse(GetType(NormalizerType), dis.readUTF())
			If type.Equals(NormalizerType.CUSTOM) Then
				' For custom serializers, the next value is a string with the class opName
				Dim strategyClassName As String = dis.readUTF()
				Dim strategyClass As Type = ND4JClassLoading.loadClassByName(strategyClassName)
				Return New Header(type, strategyClass)
			Else
				Return New Header(type, Nothing)
			End If
		End Function

		''' <summary>
		''' Write the data header
		''' </summary>
		''' <param name="stream"> the output stream </param>
		''' <param name="header"> the header to write </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeHeader(OutputStream stream, Header header) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter header was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Sub writeHeader(ByVal stream As Stream, ByVal header_Conflict As Header)
			Dim dos As New DataOutputStream(stream)
			dos.writeUTF(NormalizerSerializer.HEADER)

			' Write the current version
			dos.writeInt(1)

			' Write the normalizer opType
			dos.writeUTF(header_Conflict.normalizerType.ToString())

			' If the header contains a custom class opName, write that too
			If header_Conflict.customStrategyClass IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				dos.writeUTF(header_Conflict.customStrategyClass.FullName)
			End If
		End Sub

		''' <summary>
		''' Represents the header of a serialized normalizer file
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value private static class Header
		Private Class Header
			Friend normalizerType As NormalizerType
			Friend customStrategyClass As Type

			Public Shared Function fromStrategy(ByVal strategy As NormalizerSerializerStrategy) As Header
				If TypeOf strategy Is CustomSerializerStrategy Then
					Return New Header(strategy.getSupportedType(), strategy.GetType())
				Else
					Return New Header(strategy.getSupportedType(), Nothing)
				End If
			End Function
		End Class
	End Class

End Namespace