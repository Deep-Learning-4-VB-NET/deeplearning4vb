Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.linalg.api.blas

	Public Interface Level1
		''' <summary>
		''' computes a vector-vector dot product. </summary>
		''' <param name="n"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y">
		''' @return </param>
		Function dot(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray) As Double

		''' <summary>
		''' Vector-vector dot product </summary>
		Function dot(ByVal N As Long, ByVal dx As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer) As Double

		''' <summary>
		''' computes the Euclidean norm of a vector. </summary>
		''' <param name="arr">
		''' @return </param>
		Function nrm2(ByVal arr As INDArray) As Double

		''' <summary>
		''' computes the sum of magnitudes of all vector elements or, for a complex vector x, the sum </summary>
		''' <param name="arr">
		''' @return </param>
		Function asum(ByVal arr As INDArray) As Double

		''' <summary>
		''' sum of magnitudes of all elements </summary>
		Function asum(ByVal N As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer) As Double

		''' <summary>
		''' finds the element of a
		''' vector that has the largest absolute value. </summary>
		''' <param name="arr">
		''' @return </param>
		Function iamax(ByVal arr As INDArray) As Integer

		''' <summary>
		''' finds the element of a
		''' vector that has the largest absolute value. </summary>
		''' <param name="n"> the length to iterate for </param>
		''' <param name="arr"> the array to get the max
		'''            index for </param>
		''' <param name="stride">  the stride for the array
		''' @return </param>
		Function iamax(ByVal N As Long, ByVal arr As INDArray, ByVal stride As Integer) As Integer

		''' <summary>
		''' Index of largest absolute value </summary>
		Function iamax(ByVal N As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer) As Integer

		''' <summary>
		''' finds the element of a vector that has the minimum absolute value. </summary>
		''' <param name="arr">
		''' @return </param>
		Function iamin(ByVal arr As INDArray) As Integer

		''' <summary>
		''' swaps a vector with another vector. </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Sub swap(ByVal x As INDArray, ByVal y As INDArray)

		''' <summary>
		''' copy a vector to another vector. </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Sub copy(ByVal x As INDArray, ByVal y As INDArray)

		''' <summary>
		'''copy a vector to another vector. </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Sub copy(ByVal N As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)

		''' <summary>
		'''  computes a vector-scalar product and adds the result to a vector. </summary>
		''' <param name="n"> </param>
		''' <param name="alpha"> </param>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Sub axpy(ByVal N As Long, ByVal alpha As Double, ByVal x As INDArray, ByVal y As INDArray)

		''' <summary>
		''' computes a vector-scalar product and adds the result to a vector.
		''' y = a*x + y </summary>
		''' <param name="n"> number of operations </param>
		''' <param name="alpha"> </param>
		''' <param name="x"> X </param>
		''' <param name="offsetX"> offset of first element of X in buffer </param>
		''' <param name="incrX"> increment/stride between elements of X in buffer </param>
		''' <param name="y"> Y </param>
		''' <param name="offsetY"> offset of first element of Y in buffer </param>
		''' <param name="incrY"> increment/stride between elements of Y in buffer </param>
		Sub axpy(ByVal N As Long, ByVal alpha As Double, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)

		''' <summary>
		''' computes parameters for a Givens rotation. </summary>
		''' <param name="a"> </param>
		''' <param name="b"> </param>
		''' <param name="c"> </param>
		''' <param name="s"> </param>
		Sub rotg(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal s As INDArray)

		''' <summary>
		''' performs rotation of points in the plane. </summary>
		''' <param name="N"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="c"> </param>
		''' <param name="s"> </param>
		Sub rot(ByVal N As Long, ByVal X As INDArray, ByVal Y As INDArray, ByVal c As Double, ByVal s As Double)

		''' <summary>
		''' computes the modified parameters for a Givens rotation. </summary>
		''' <param name="d1"> </param>
		''' <param name="d2"> </param>
		''' <param name="b1"> </param>
		''' <param name="b2"> </param>
		''' <param name="P"> </param>
		Sub rotmg(ByVal d1 As INDArray, ByVal d2 As INDArray, ByVal b1 As INDArray, ByVal b2 As Double, ByVal P As INDArray)

		''' <summary>
		'''  computes a vector by a scalar product. </summary>
		''' <param name="N"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		Sub scal(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray)


		''' <summary>
		''' Can we use the axpy and copy methods that take a DataBuffer instead of an INDArray with this backend? </summary>
		Function supportsDataBufferL1Ops() As Boolean
	End Interface

End Namespace