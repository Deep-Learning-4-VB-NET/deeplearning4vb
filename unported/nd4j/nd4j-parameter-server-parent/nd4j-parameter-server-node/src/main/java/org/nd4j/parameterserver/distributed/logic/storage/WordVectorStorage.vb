Imports System
Imports BaseStorage = org.nd4j.parameterserver.distributed.logic.storage.BaseStorage

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

Namespace org.nd4j.parameterserver.distributed.logic.storage

	<Obsolete>
	Public Class WordVectorStorage
		Inherits BaseStorage

		Public Shared ReadOnly SYN_0 As Integer? = "syn0".GetHashCode()
		Public Shared ReadOnly SYN_1 As Integer? = "syn1".GetHashCode()
		Public Shared ReadOnly SYN_1_NEGATIVE As Integer? = "syn1Neg".GetHashCode()
		Public Shared ReadOnly EXP_TABLE As Integer? = "expTable".GetHashCode()
		Public Shared ReadOnly NEGATIVE_TABLE As Integer? = "negTable".GetHashCode()
	End Class

End Namespace