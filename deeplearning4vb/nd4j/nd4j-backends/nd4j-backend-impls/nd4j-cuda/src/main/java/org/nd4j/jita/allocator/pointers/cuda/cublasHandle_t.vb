﻿Imports Pointer = org.bytedeco.javacpp.Pointer
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer

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

Namespace org.nd4j.jita.allocator.pointers.cuda

	''' <summary>
	''' Created by raver119 on 19.04.16.
	''' </summary>
	Public Class cublasHandle_t
		Inherits CudaPointer

		Public Sub New(ByVal pointer As Pointer)
			MyBase.New(pointer)
		End Sub
	End Class

End Namespace