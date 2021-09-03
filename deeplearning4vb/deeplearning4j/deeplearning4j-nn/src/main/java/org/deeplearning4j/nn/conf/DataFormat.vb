Imports DataFormatDeserializer = org.deeplearning4j.nn.conf.serde.format.DataFormatDeserializer
Imports DataFormatSerializer = org.deeplearning4j.nn.conf.serde.format.DataFormatSerializer
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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
Namespace org.deeplearning4j.nn.conf

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = DataFormatSerializer.class) @JsonDeserialize(using = DataFormatDeserializer.class) public interface DataFormat
	Public Interface DataFormat
	End Interface

End Namespace