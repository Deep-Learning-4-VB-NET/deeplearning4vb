﻿Imports System
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize

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

Namespace org.deeplearning4j.nn.conf.distribution.serde

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonDeserialize(using = LegacyDistributionDeserializer.class) public class LegacyDistributionHelper extends org.deeplearning4j.nn.conf.distribution.Distribution
	<Serializable>
	Public Class LegacyDistributionHelper
		Inherits Distribution

		Private Sub New()

		End Sub

	End Class

End Namespace