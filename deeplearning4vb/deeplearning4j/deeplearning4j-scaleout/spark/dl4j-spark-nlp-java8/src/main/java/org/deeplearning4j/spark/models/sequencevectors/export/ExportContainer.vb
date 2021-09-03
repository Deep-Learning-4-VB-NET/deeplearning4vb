Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.spark.models.sequencevectors.export


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor @Builder public class ExportContainer<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements java.io.Serializable
	<Serializable>
	Public Class ExportContainer(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Private element As T
		Private array As INDArray

		Protected Friend Shared ReadOnly pattern0 As Pattern = Pattern.compile("(\[|\])")
		Protected Friend Shared ReadOnly pattern1 As Pattern = Pattern.compile("(\,|\.|\;)+\s")

		' TODO: implement B64 optional compression here?
		Public Overrides Function ToString() As String
			' TODO: we need proper string cleansing here

			Dim ars As String = Arrays.toString(array.data().asFloat())
			ars = pattern0.matcher(ars).replaceAll("").Trim()
			ars = pattern1.matcher(ars).replaceAll(" ").Trim()

			Return element.Label.Trim() & " " & ars
		End Function
	End Class

End Namespace