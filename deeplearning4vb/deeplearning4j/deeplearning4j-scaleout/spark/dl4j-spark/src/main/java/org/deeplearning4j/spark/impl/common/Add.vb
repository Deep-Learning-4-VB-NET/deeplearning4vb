Imports Function2 = org.apache.spark.api.java.function.Function2
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.spark.impl.common

	''' <summary>
	''' Adds 2 ndarrays
	''' @author Adam Gibson
	''' </summary>
	Public Class Add
		Implements Function2(Of INDArray, INDArray, INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray call(org.nd4j.linalg.api.ndarray.INDArray v1, org.nd4j.linalg.api.ndarray.INDArray v2) throws Exception
		Public Overrides Function [call](ByVal v1 As INDArray, ByVal v2 As INDArray) As INDArray
			Dim res As INDArray = v1.addi(v2)

			Nd4j.Executioner.commit()

			Return res
		End Function
	End Class

End Namespace