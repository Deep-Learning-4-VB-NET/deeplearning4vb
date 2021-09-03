Imports Function2 = org.apache.spark.api.java.function.Function2
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

Namespace org.deeplearning4j.spark.impl.common.reduce

	Public Class LongDoubleReduceFunction
		Implements Function2(Of Tuple2(Of Long, Double), Tuple2(Of Long, Double), Tuple2(Of Long, Double))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<Long, Double> call(scala.Tuple2<Long, Double> f, scala.Tuple2<Long, Double> s) throws Exception
		Public Overrides Function [call](ByVal f As Tuple2(Of Long, Double), ByVal s As Tuple2(Of Long, Double)) As Tuple2(Of Long, Double)
			Return New Tuple2(Of Long, Double)(f._1() + s._1(), f._2() + s._2())
		End Function
	End Class

End Namespace