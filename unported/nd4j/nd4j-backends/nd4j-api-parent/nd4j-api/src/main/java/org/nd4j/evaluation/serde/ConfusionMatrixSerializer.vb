Imports System.Collections.Generic
Imports Multiset = org.nd4j.shade.guava.collect.Multiset
Imports org.nd4j.evaluation.classification
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports JsonSerializer = org.nd4j.shade.jackson.databind.JsonSerializer
Imports SerializerProvider = org.nd4j.shade.jackson.databind.SerializerProvider

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

Namespace org.nd4j.evaluation.serde


	Public Class ConfusionMatrixSerializer
		Inherits JsonSerializer(Of ConfusionMatrix(Of Integer))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.nd4j.evaluation.classification.ConfusionMatrix<Integer> cm, org.nd4j.shade.jackson.core.JsonGenerator gen, org.nd4j.shade.jackson.databind.SerializerProvider provider) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overrides Sub serialize(ByVal cm As ConfusionMatrix(Of Integer), ByVal gen As JsonGenerator, ByVal provider As SerializerProvider)
			Dim classes As IList(Of Integer) = cm.getClasses()
			Dim matrix As IDictionary(Of Integer, Multiset(Of Integer)) = cm.getMatrix()

			Dim m2 As IDictionary(Of Integer, Integer()()) = New LinkedHashMap(Of Integer, Integer()())()
			For Each i As Integer? In matrix.Keys 'i = Actual class
				Dim ms As Multiset(Of Integer) = matrix(i)
				Dim arr()() As Integer = { New Integer(ms.size() - 1){}, New Integer(ms.size() - 1){} }
				Dim used As Integer = 0
				For Each j As Integer? In ms.elementSet()
					Dim count As Integer = ms.count(j)
					arr(0)(used) = j 'j = Predicted class
					arr(1)(used) = count 'prediction count
					used += 1
				Next j
				m2(i) = arr
			Next i

			gen.writeStartObject()
			gen.writeObjectField("classes", classes)
			gen.writeObjectField("matrix", m2)
			gen.writeEndObject()
		End Sub
	End Class

End Namespace