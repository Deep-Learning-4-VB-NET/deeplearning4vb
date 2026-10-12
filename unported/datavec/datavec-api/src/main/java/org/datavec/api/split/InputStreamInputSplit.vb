Imports System
Imports System.Collections.Generic
Imports System.IO

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


	Public Class InputStreamInputSplit
		Implements InputSplit

'JAVA TO VB CONVERTER NOTE: The field is was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private is_Conflict As Stream
		Private location() As URI

		''' <summary>
		''' Instantiate with the given
		''' file as a uri </summary>
		''' <param name="is"> the input stream to use </param>
		''' <param name="path"> the path to use </param>
		Public Sub New(ByVal [is] As Stream, ByVal path As String)
			Me.New([is], URI.create(path))
		End Sub

		''' <summary>
		''' Instantiate with the given
		''' file as a uri </summary>
		''' <param name="is"> the input stream to use </param>
		''' <param name="path"> the path to use </param>
		Public Sub New(ByVal [is] As Stream, ByVal path As File)
			Me.New([is], path.toURI())
		End Sub

		''' <summary>
		''' Instantiate with the given
		''' file as a uri </summary>
		''' <param name="is"> the input stream to use </param>
		''' <param name="path"> the path to use </param>
		Public Sub New(ByVal [is] As Stream, ByVal path As URI)
			Me.is_Conflict = [is]
			Me.location = New URI() {path}
		End Sub


		Public Sub New(ByVal [is] As Stream)
			Me.is_Conflict = [is]
			Me.location = New URI(){}
		End Sub

		Public Overridable Function canWriteToLocation(ByVal location As URI) As Boolean Implements InputSplit.canWriteToLocation
			Return False
		End Function

		Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
			Return Nothing
		End Function

		Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
			Return Nothing
		End Function

		Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations

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
			Return is_Conflict
		End Function

		Public Overridable Function length() As Long Implements InputSplit.length
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function locations() As URI() Implements InputSplit.locations
			Return location
		End Function

		Public Overridable Function locationsIterator() As IEnumerator(Of URI) Implements InputSplit.locationsIterator
			Return Collections.singletonList(location(0)).GetEnumerator()
		End Function

		Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
			If location.Length >= 1 Then
				Return Collections.singletonList(location(0).getPath()).GetEnumerator()
			End If
			Return Arrays.asList("").GetEnumerator()
		End Function

		Public Overridable Sub reset() Implements InputSplit.reset
			If Not resetSupported() Then
				Throw New System.NotSupportedException("Reset not supported from streams")
			End If
			Try
				is_Conflict = openInputStreamFor(location(0).getPath())
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
			Return location IsNot Nothing AndAlso location.Length > 0
		End Function


		Public Overridable Property Is As Stream
			Get
				Return is_Conflict
			End Get
			Set(ByVal [is] As Stream)
				Me.is_Conflict = [is]
			End Set
		End Property


	End Class

End Namespace