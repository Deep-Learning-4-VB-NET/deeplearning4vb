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


	''' <summary>
	''' String split used for single line inputs
	''' @author Adam Gibson
	''' </summary>
	Public Class StringSplit
		Implements InputSplit

'JAVA TO VB CONVERTER NOTE: The field data was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private data_Conflict As String

		Public Sub New(ByVal data As String)
			Me.data_Conflict = data
		End Sub

		Public Overridable Function canWriteToLocation(ByVal location As URI) As Boolean Implements InputSplit.canWriteToLocation
			Return True
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
			Return False
		End Function

		Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.OutputStream openOutputStreamFor(String location) throws Exception
		Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
			Return Nothing
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.InputStream openInputStreamFor(String location) throws Exception
		Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
			Return Nothing
		End Function

		Public Overridable Function length() As Long Implements InputSplit.length
			Return data_Conflict.Length
		End Function

		Public Overridable Function locations() As URI() Implements InputSplit.locations
			Return New URI(){}
		End Function

		Public Overridable Function locationsIterator() As IEnumerator(Of URI) Implements InputSplit.locationsIterator
			Return Collections.emptyIterator()
		End Function

		Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
			Return Collections.emptyIterator()
		End Function

		Public Overridable Sub reset() Implements InputSplit.reset
			'No op
		End Sub

		Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
			Return True
		End Function



		Public Overridable ReadOnly Property Data As String
			Get
				Return data_Conflict
			End Get
		End Property

	End Class

End Namespace