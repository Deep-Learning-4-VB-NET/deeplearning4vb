﻿Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.imports.graphmapper.tf.tensors


	''' @param <J> Java array type </param>
	''' @param <B> Java buffer type </param>
	Public Interface TFTensorMapper(Of J, B As java.nio.Buffer)

		Friend Enum ValueSource
			EMPTY
			VALUE_COUNT
			BINARY

		End Enum
		Function dataType() As DataType

		Function shape() As Long()

		ReadOnly Property Empty As Boolean

		Function valueSource() As ValueSource

		Function valueCount() As Integer

		Function newArray(ByVal length As Integer) As J

		Function getBuffer(ByVal bb As ByteBuffer) As B

		Function toNDArray() As INDArray

		Sub getValue(ByVal jArr As J, ByVal i As Integer)

		Sub getValue(ByVal jArr As J, ByVal buffer As B, ByVal i As Integer)

		Function arrayFor(ByVal shape() As Long, ByVal jArr As J) As INDArray


	End Interface

End Namespace