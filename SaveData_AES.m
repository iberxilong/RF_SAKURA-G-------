clear;
clc;
%保存AES的数据
name='附件8：数缘科技 - SAKURA-G开发板采波软件\DSO-X 3034T WaveData 2021_11_17_19h12m50s\';
filetime='DSO-X 3034T WaveData 2021_11_17_19h12m50s';
C=[];
key=[];
cipher=[];
plaintext=[];
traces=[];
% AA=[];
% CC=[];

fid = fopen(fullfile(['E:\',name,'Atext.csv']), 'r');  %更换一下文件位置 

tracesNumber=10016;
for numtraces=1:tracesNumber
    % % 密钥提取
    clear d;
    keylen=128;
    d(1,:) = fgets(fid);
    e(1,:)=fgets(fid);
    G=[];
    for k=1:1:keylen/4
        
        % f=hex2dec(e(k));
        g=dec2bin(hex2dec(e(k)));
        g1=str2num(g);
        %disp(fliplr(g1));
        g2=num2str(g1,'%04d');
        % fprintf("g2=%s\n",g2);
        g3=flipud(g2')';
        % fprintf("g3=%s\n",g3);
        %disp(g2);
        G=[g3,G];
        % fprintf("G=%s\n",G);
        clear g g1 g2 g3 
    end
    key1=[];
    for m=1:1:keylen
       key1(1,m)=str2double(G(1,m));
    end
    key=[key,key1'];
%     COUT(key1');

    %%明文读取
    clear f g;
    f(1,:)=fgets(fid);
    plaintextlen=128;
    G=[];
    for k=1:1:plaintextlen/4
        g=dec2bin(hex2dec(f(k)));
        g1=str2num(g);
        g2=num2str(g1,'%04d');
        g3=flipud(g2')';
        G=[g3,G];
    clear g g1 g2 g3
    end
    plaintext1=[];
    for m=1:1:plaintextlen
       plaintext1(1,m)=str2double(G(1,m));
    end
    plaintext=[plaintext,plaintext1'];
%     COUT(plaintext1');
    
    %%密文读取
    f1(1,:)=fgets(fid);
    f(1,:)=fgets(fid);
    cipherlen=128;
    G=[];
    for k=1:1:cipherlen/4
    g=dec2bin(hex2dec(f(k)));
    g1=str2num(g);
    g2=num2str(g1,'%04d');
    g3=flipud(g2')';
    G=[g3,G];
    clear g g1 g2 g3 e 
    end
    cipher1=[];
    for m=1:1:cipherlen
       cipher1(1,m)=str2double(G(1,m));
    end
    cipher=[cipher,cipher1'];
%     COUT(cipher1');
    
%     fprintf('k=%d\n',numtraces);
    


    % %波形数据的提取
    fileName1=['E:\',name,'#' num2str(numtraces) '#.csv'];   %更换一下文件位置
     A=csvread(fileName1,0,0);%只存一行traces
    traces=[traces,A(:,1)];
    
    if(numtraces<=8)%第一条通常有问题，不要(所以要去掉前4条！！)
        key=[];
        cipher=[];
        plaintext=[];
        traces=[];
    end
end
fclose(fid);
% traces=traces(400:1600,:);

%trigger=CC';
fclose all;
save(['E:\附件8：数缘科技 - SAKURA-G开发板采波软件\',filetime,'.mat'],'traces','key','cipher','plaintext','-v7.3');
%clearvars -except coff cipher key traces trigger

% function COUT(data)
% %16进制key显示
%     n=size(data,1);
%     t=flipud(data);   
%     for ii=1:1:n/4
%          a=num2str(t(4*(ii-1)+1:ii*4))';
%          %fprintf("bin2dec=%d\n",bin2dec(a));
%          a=bin2dec(a);
%          a=dec2hex(a);
%          fprintf("%c",a);
%     end
%     fprintf("\n");
% end

