Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter

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


	Public Class OutputStreamInputSplit
		Implements InputSplit

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private java.io.OutputStream outputStream;
		Private outputStream As Stream


		Public Sub New(ByVal outputStream As Stream)
			Me.outputStream = outputStream
		End Sub

		Public Overridable Function canWriteToLocation(ByVal location As URI) As Boolean Implements InputSplit.canWriteToLocation
			Return True
		End Function

		Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
			Return Nothing
		End Function

		Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
			Throw New System.NotSupportedException()

		End Sub

		Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
			Return False
		End Function

		Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.OutputStream openOutputStreamFor(String location) throws Exception
		Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
			Return outputStream
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.InputStream openInputStreamFor(String location) throws Exception
		Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function length() As Long Implements InputSplit.length
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function locations() As URI() Implements InputSplit.locations
			Return New URI(){}

		End Function

		Public Overridable Function locationsIterator() As IEnumerator(Of URI) Implements InputSplit.locationsIterator
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
			Throw New System.NotSupportedException()

		End Function

		Public Overridable Sub reset() Implements InputSplit.reset
			'No op
			If Not resetSupported() Then
				Throw New System.NotSupportedException("Reset not supported from streams")
			End If
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
			Return False
		End Function


	End Class

End Namespace