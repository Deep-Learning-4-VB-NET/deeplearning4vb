﻿'
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

	Public Enum ArrayType
		''' <summary>
		''' This means DistributedINDArray will be equal on all ends, and will never be modified after replication/instantiation
		''' </summary>
		CONSTANT

		''' <summary>
		''' This means VariadicINDArray will have exactly the same data type and shape on different, thus entries can have synchronized values
		''' </summary>
		SYNCABLE

		''' <summary>
		''' This means DistributedINDArray might (or might not) have different shapes on different entries
		''' </summary>
		VARIADIC
	End Enum

End Namespace