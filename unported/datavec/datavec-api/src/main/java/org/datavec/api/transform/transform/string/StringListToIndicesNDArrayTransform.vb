Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.datavec.api.transform.transform.string


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class StringListToIndicesNDArrayTransform extends StringListToCountsNDArrayTransform
	<Serializable>
	Public Class StringListToIndicesNDArrayTransform
		Inherits StringListToCountsNDArrayTransform

		''' <param name="columnName">     The name of the column to convert </param>
		''' <param name="vocabulary">     The possible tokens that may be present. </param>
		''' <param name="delimiter">      The delimiter for the Strings to convert </param>
		''' <param name="ignoreUnknown">  Whether to ignore unknown tokens </param>
		Public Sub New(ByVal columnName As String, ByVal vocabulary As IList(Of String), ByVal delimiter As String, ByVal binary As Boolean, ByVal ignoreUnknown As Boolean)
			MyBase.New(columnName, vocabulary, delimiter, binary, ignoreUnknown)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringListToIndicesNDArrayTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newColumnName") String newColumnName, @JsonProperty("vocabulary") java.util.List<String> vocabulary, @JsonProperty("delimiter") String delimiter, @JsonProperty("binary") boolean binary, @JsonProperty("ignoreUnknown") boolean ignoreUnknown)
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal vocabulary As IList(Of String), ByVal delimiter As String, ByVal binary As Boolean, ByVal ignoreUnknown As Boolean)
			MyBase.New(columnName, newColumnName, vocabulary, delimiter, binary, ignoreUnknown)
		End Sub

		Protected Friend Overrides Function makeBOWNDArray(ByVal indices As ICollection(Of Integer)) As INDArray
			Dim counts As INDArray = Nd4j.zeros(1, indices.Count)
			Dim indicesSorted As IList(Of Integer) = New List(Of Integer)(indices)
			indicesSorted.Sort()
			For i As Integer = 0 To indicesSorted.Count - 1
				counts.putScalar(i, indicesSorted(i))
			Next i
			Nd4j.Executioner.commit()
			Return counts
		End Function
	End Class

End Namespace