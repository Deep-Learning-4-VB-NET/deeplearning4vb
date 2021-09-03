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

Namespace org.nd4j.linalg.api.ops

	Public Interface ScalarOp
		Inherits Op

		''' <summary>
		'''The normal scalar </summary>
		''' <returns> the scalar </returns>
		Function scalar() As INDArray

		''' <summary>
		''' This method allows to set scalar </summary>
		''' <param name="scalar"> </param>
		WriteOnly Property Scalar As Number

		WriteOnly Property Scalar As INDArray

		''' <summary>
		''' This method returns target dimensions for this op
		''' @return
		''' </summary>
		Function dimensions() As INDArray

		Property Dimension As Integer()


		Function validateDataTypes(ByVal experimentalMode As Boolean) As Boolean

		ReadOnly Property OpType As Type
	End Interface

End Namespace