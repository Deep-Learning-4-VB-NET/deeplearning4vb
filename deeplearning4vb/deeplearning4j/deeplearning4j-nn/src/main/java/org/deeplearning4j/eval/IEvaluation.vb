﻿Imports System
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.eval

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY) public interface IEvaluation<T extends org.nd4j.evaluation.IEvaluation> extends org.nd4j.evaluation.IEvaluation<T>
	<Obsolete>
	Public Interface IEvaluation(Of T As org.nd4j.evaluation.IEvaluation)
		Inherits org.nd4j.evaluation.IEvaluation(Of T)

	End Interface

End Namespace