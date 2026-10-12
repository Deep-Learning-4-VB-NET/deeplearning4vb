Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data public class LSTMCellConfiguration
	Public Class LSTMCellConfiguration
		''' <summary>
		'''   NDArray<T>* xt   = INPUT_VARIABLE(0);                   // input [batchSize x inSize]
		''' NDArray<T>* ht_1 = INPUT_VARIABLE(1);                   // previous cell output [batchSize x numProj],  that is at previous time step t-1, in case of projection=false -> numProj=numUnits!!!
		''' NDArray<T>* ct_1 = INPUT_VARIABLE(2);                   // previous cell state  [batchSize x numUnits], that is at previous time step t-1
		''' 
		''' NDArray<T>* Wx   = INPUT_VARIABLE(3);                   // input-to-hidden  weights, [inSize  x 4*numUnits]
		''' NDArray<T>* Wh   = INPUT_VARIABLE(4);                   // hidden-to-hidden weights, [numProj x 4*numUnits]
		''' NDArray<T>* Wc   = INPUT_VARIABLE(5);                   // diagonal weights for peephole connections [1 x 3*numUnits]
		''' NDArray<T>* Wp   = INPUT_VARIABLE(6);                   // projection weights [numUnits x numProj]
		''' NDArray<T>* b    = INPUT_VARIABLE(7);                   // biases, [1 x 4*numUnits]
		''' 
		''' NDArray<T>* ht   =  OUTPUT_VARIABLE(0);                  // current cell output [batchSize x numProj], that is at current time step t
		''' NDArray<T>* ct   =  OUTPUT_VARIABLE(1);                  // current cell state  [batchSize x numUnits], that is at current time step t
		''' 
		''' const bool peephole   = (bool)INT_ARG(0);               // if true, provide peephole connections
		''' const bool projection = (bool)INT_ARG(1);               // if true, then projection is performed, if false then numProj==numUnits is mandatory!!!!
		''' T clippingCellValue   = T_ARG(0);                       // clipping value for ct, if it is not equal to zero, then cell state is clipped
		''' T clippingProjValue   = T_ARG(1);                       // clipping value for projected ht, if it is not equal to zero, then projected cell output is clipped
		''' const T forgetBias    = T_ARG(2);
		''' 
		''' </summary>

		Private peepHole As Boolean
		Private projection As Boolean
		Private clippingCellValue As Double
		Private clippingProjValue As Double
		Private forgetBias As Double
		'input variables
		Private xt, ht_1, ct_1, Wx, Wh, Wc, Wp, b As SDVariable

		Public Overridable Function toProperties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("peepHole") = peepHole
			ret("projection") = projection
			ret("clippingCellValue") = clippingCellValue
			ret("clippingProjValue") = clippingProjValue
			ret("forgetBias") = forgetBias
			Return ret
		End Function


		Public Overridable Function args() As SDVariable()
			Return New SDVariable() {xt, ht_1, ct_1, Wx, Wh, Wc, Wp, b}
		End Function


		Public Overridable Function iArgs() As Integer()
			Return New Integer() {ArrayUtil.fromBoolean(peepHole), ArrayUtil.fromBoolean(projection)}
		End Function

		Public Overridable Function tArgs() As Double()
			Return New Double() {clippingCellValue, clippingProjValue, forgetBias}
		End Function


	End Class

End Namespace