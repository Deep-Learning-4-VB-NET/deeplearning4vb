Imports System
Imports Data = lombok.Data
Imports Writable = org.datavec.api.writable.Writable
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.sequence.comparator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class StringComparator extends BaseColumnComparator
	<Serializable>
	Public Class StringComparator
		Inherits BaseColumnComparator

		''' <param name="columnName"> Name of the column in which to compare values </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringComparator(@JsonProperty("columnName") String columnName)
		Public Sub New(ByVal columnName As String)
			MyBase.New(columnName)
		End Sub

		Protected Friend Overrides Function Compare(ByVal w1 As Writable, ByVal w2 As Writable) As Integer
			Return String.CompareOrdinal(w1.ToString(), w2.ToString())
		End Function
	End Class

End Namespace