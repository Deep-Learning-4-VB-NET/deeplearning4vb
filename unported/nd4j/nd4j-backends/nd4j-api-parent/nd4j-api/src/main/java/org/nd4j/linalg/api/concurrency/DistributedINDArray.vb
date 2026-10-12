Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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

Namespace org.nd4j.linalg.api.concurrency

	Public Interface DistributedINDArray

		''' <summary>
		''' This method returns ArrayType for this instance
		''' @return
		''' </summary>
		ReadOnly Property INDArrayType As ArrayType

		''' <summary>
		''' This method returns INDArray for specific entry (i.e. for specific device, if you put entries that way)
		''' </summary>
		''' <param name="entry">
		''' @return </param>
		Function entry(ByVal entry As Integer) As INDArray

		''' <summary>
		''' This method returns INDArray for the current device
		''' 
		''' PLEASE NOTE: if you use more than one thread per device you'd better not use this method unless you're 100% sure
		''' @return
		''' </summary>
		Function entry() As INDArray

		''' <summary>
		''' This method propagates given INDArray to all entries as is
		''' </summary>
		''' <param name="array"> </param>
		Sub propagate(ByVal array As INDArray)

		''' <summary>
		''' This method returns total number of entries within this DistributedINDArray instance
		''' @return
		''' </summary>
		Function numEntries() As Integer

		''' <summary>
		''' This method returns number of activated entries
		''' @return
		''' </summary>
		Function numActiveEntries() As Integer

		''' <summary>
		''' This method allocates INDArray for specified entry
		''' </summary>
		''' <param name="entry"> </param>
		''' <param name="shapeDescriptor"> </param>
		Sub allocate(ByVal entry As Integer, ByVal shapeDescriptor As LongShapeDescriptor)

		''' <summary>
		''' This method allocates INDArray for specified entry
		''' </summary>
		''' <param name="entry"> </param>
		Sub allocate(ByVal entry As Integer, ByVal dataType As DataType, ParamArray ByVal shape() As Long)
	End Interface

End Namespace