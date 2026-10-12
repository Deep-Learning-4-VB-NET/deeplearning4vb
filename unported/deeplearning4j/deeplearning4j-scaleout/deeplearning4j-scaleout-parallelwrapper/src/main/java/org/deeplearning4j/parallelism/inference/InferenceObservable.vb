Imports System
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.parallelism.inference


	Public Interface InferenceObservable

		''' <summary>
		''' Get input batches - and their associated input mask arrays, if any<br>
		''' Note that usually the returned list will be of size 1 - however, in the batched case, not all inputs
		''' can actually be batched (variable size inputs to fully convolutional net, for example). In these "can't batch"
		''' cases, multiple input batches will be returned, to be processed
		''' </summary>
		''' <returns> List of pairs of input arrays and input mask arrays. Input mask arrays may be null. </returns>
		ReadOnly Property InputBatches As IList(Of Pair(Of INDArray(), INDArray()))

		Sub addInput(ParamArray ByVal input() As INDArray)

		Sub addInput(ByVal input() As INDArray, ByVal inputMasks() As INDArray)

		WriteOnly Property OutputBatches As IList(Of INDArray())

		WriteOnly Property OutputException As Exception

		Sub addObserver(ByVal observer As Observer)

		ReadOnly Property Output As INDArray()
	End Interface

End Namespace