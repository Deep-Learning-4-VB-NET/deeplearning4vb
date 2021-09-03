Imports Function2 = org.apache.spark.api.java.function.Function2
Imports org.nd4j.evaluation

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

Namespace org.deeplearning4j.spark.impl.multilayer.evaluation

	Public Class IEvaluateAggregateFunction(Of T As org.nd4j.evaluation.IEvaluation)
		Implements Function2(Of T(), T(), T())

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public T[] call(T[] v1, T[] v2) throws Exception
		Public Overrides Function [call](ByVal v1() As T, ByVal v2() As T) As T()
			If v1 Is Nothing Then
				Return v2
			End If
			If v2 Is Nothing Then
				Return v1
			End If
			For i As Integer = 0 To v1.Length - 1
				v1(i).merge(v2(i))
			Next i
			Return v1
		End Function
	End Class

End Namespace