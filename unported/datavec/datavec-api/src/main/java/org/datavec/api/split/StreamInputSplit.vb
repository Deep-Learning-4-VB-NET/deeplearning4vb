Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports org.datavec.api.util.files
Imports org.nd4j.common.function
Imports MathUtils = org.nd4j.common.util.MathUtils

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

Namespace org.datavec.api.split


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class StreamInputSplit implements InputSplit
	Public Class StreamInputSplit
		Implements InputSplit

		Protected Friend uris As IList(Of URI)
		Protected Friend streamCreatorFn As [Function](Of URI, Stream)
		Protected Friend rng As Random
		Protected Friend order() As Integer

		''' <summary>
		''' Create a StreamInputSplit with no randomization
		''' </summary>
		''' <param name="uris">            The list of URIs to load </param>
		''' <param name="streamCreatorFn"> The function to be used to create InputStream objects for a given URI. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StreamInputSplit(@NonNull List<java.net.URI> uris, @NonNull @Function<java.net.URI,java.io.InputStream> streamCreatorFn)
		Public Sub New(ByVal uris As IList(Of URI), ByVal streamCreatorFn As [Function](Of URI, Stream))
			Me.New(uris, streamCreatorFn, Nothing)
		End Sub

		''' <summary>
		''' Create a StreamInputSplit with optional randomization
		''' </summary>
		''' <param name="uris">            The list of URIs to load </param>
		''' <param name="streamCreatorFn"> The function to be used to create InputStream objects for a given URI </param>
		''' <param name="rng">             Random number generator instance. If non-null: streams will be iterated over in a random
		'''                        order. If null: no randomization (iteration order is according to the URIs list) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StreamInputSplit(@NonNull List<java.net.URI> uris, @NonNull @Function<java.net.URI,java.io.InputStream> streamCreatorFn, Random rng)
		Public Sub New(ByVal uris As IList(Of URI), ByVal streamCreatorFn As [Function](Of URI, Stream), ByVal rng As Random)
			Me.uris = uris
			Me.streamCreatorFn = streamCreatorFn
			Me.rng = rng
		End Sub

		Public Overridable Function canWriteToLocation(ByVal location As URI) As Boolean Implements InputSplit.canWriteToLocation
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
			Throw New System.NotSupportedException()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.OutputStream openOutputStreamFor(String location) throws Exception
		Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.InputStream openInputStreamFor(String location) throws Exception
		Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function length() As Long Implements InputSplit.length
			Return uris.Count
		End Function

		Public Overridable Function locations() As URI() Implements InputSplit.locations
			Return CType(uris, List(Of URI)).ToArray()
		End Function

		Public Overridable Function locationsIterator() As IEnumerator(Of URI)
			If rng Is Nothing Then
				Return uris.GetEnumerator()
			Else
				If order Is Nothing Then
					order = New Integer(uris.Count - 1){}
					For i As Integer = 0 To order.Length - 1
						order(i) = i
					Next i
				End If
				MathUtils.shuffleArray(order, rng)
				Return New ShuffledListIterator(Of URI)(uris, order)
			End If
		End Function

		Public Overridable Function locationsPathIterator() As IEnumerator(Of String)
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub reset() Implements InputSplit.reset
			'No op
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
			Return True
		End Function
	End Class

End Namespace