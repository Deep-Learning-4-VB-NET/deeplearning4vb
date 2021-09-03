Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports ComputationGraphUtil = org.deeplearning4j.nn.graph.util.ComputationGraphUtil
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.spark.impl.graph.dataset

	Public Class PairDataSetToMultiDataSetFn(Of K)
		Implements PairFunction(Of Tuple2(Of K, DataSet), K, MultiDataSet)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<K, org.nd4j.linalg.dataset.api.MultiDataSet> call(scala.Tuple2<K, org.nd4j.linalg.dataset.DataSet> in) throws Exception
		Public Overrides Function [call](ByVal [in] As Tuple2(Of K, DataSet)) As Tuple2(Of K, MultiDataSet)
			Return New Tuple2(Of K, MultiDataSet)([in]._1(), ComputationGraphUtil.toMultiDataSet([in]._2()))
		End Function
	End Class

End Namespace