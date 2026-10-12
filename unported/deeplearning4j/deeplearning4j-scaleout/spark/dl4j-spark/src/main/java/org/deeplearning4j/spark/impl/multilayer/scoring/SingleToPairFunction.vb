Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.impl.multilayer.scoring

	Public Class SingleToPairFunction(Of T)
		Implements PairFunction(Of Tuple2(Of T, INDArray), T, Tuple2(Of INDArray, INDArray))


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<T, scala.Tuple2<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray>> call(scala.Tuple2<T, org.nd4j.linalg.api.ndarray.INDArray> t2) throws Exception
		Public Overrides Function [call](ByVal t2 As Tuple2(Of T, INDArray)) As Tuple2(Of T, Tuple2(Of INDArray, INDArray))
			Return New Tuple2(Of T, Tuple2(Of INDArray, INDArray))(t2._1(), New Tuple2(Of INDArray, INDArray)(t2._2(), Nothing))
		End Function
	End Class

End Namespace