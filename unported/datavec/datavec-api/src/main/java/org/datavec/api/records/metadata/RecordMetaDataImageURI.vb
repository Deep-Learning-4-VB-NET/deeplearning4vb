Imports System
Imports Data = lombok.Data

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

Namespace org.datavec.api.records.metadata

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class RecordMetaDataImageURI extends RecordMetaDataURI
	<Serializable>
	Public Class RecordMetaDataImageURI
		Inherits RecordMetaDataURI

		Private origC As Integer
		Private origH As Integer
		Private origW As Integer

		Public Sub New(ByVal uri As URI, ByVal readerClass As Type, ByVal origC As Integer, ByVal origH As Integer, ByVal origW As Integer)
			MyBase.New(uri, readerClass)
			Me.origC = origC
			Me.origH = origH
			Me.origW = origW
		End Sub
	End Class

End Namespace