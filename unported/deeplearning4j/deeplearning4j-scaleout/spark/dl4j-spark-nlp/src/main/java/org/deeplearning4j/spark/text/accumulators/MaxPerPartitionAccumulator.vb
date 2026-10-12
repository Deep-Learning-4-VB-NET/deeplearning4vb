Imports AccumulatorParam = org.apache.spark.AccumulatorParam
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.spark.text.accumulators

	''' <summary>
	''' @author jeffreytang
	''' </summary>
	Public Class MaxPerPartitionAccumulator
		Implements AccumulatorParam(Of Counter(Of Integer))

		Public Overrides Function addInPlace(ByVal c1 As Counter(Of Integer), ByVal c2 As Counter(Of Integer)) As Counter(Of Integer)
			c1.incrementAll(c2)
			Return c1
		End Function

		Public Overrides Function zero(ByVal initialCounter As Counter(Of Integer)) As Counter(Of Integer)
			Return New Counter(Of Integer)()
		End Function

		Public Overrides Function addAccumulator(ByVal c1 As Counter(Of Integer), ByVal c2 As Counter(Of Integer)) As Counter(Of Integer)
			If c1 Is Nothing Then
				Return New Counter(Of Integer)()
			End If
			addInPlace(c1, c2)
			Return c1
		End Function
	End Class

End Namespace